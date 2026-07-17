const API_BASE_URL = 'http://localhost:5080';

// Historical Enigma keyboard / lampboard layout
const BOARD_ROWS = [
    ['Q', 'W', 'E', 'R', 'T', 'Z', 'U', 'I', 'O'],
    ['A', 'S', 'D', 'F', 'G', 'H', 'J', 'K'],
    ['P', 'Y', 'X', 'C', 'V', 'B', 'N', 'M', 'L']
];

const ROTOR_TYPES = ['I', 'II', 'III', 'IV', 'V'];
const ALPHABET = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ';
const MAX_CABLES = 10;
const CABLE_COLORS = [
    '#c0392b', '#2980b9', '#27ae60', '#d4a017', '#8e44ad',
    '#d35400', '#16a085', '#7f8c8d', '#c2185b', '#5d4037',
    '#33691e', '#455a64', '#b8860b'
];
// Characters the text transformer understands besides letters and whitespace
const ALLOWED_PUNCTUATION = '.:?,/-()';

// ---------- Machine state (all arrays are LEFT to RIGHT, as on the machine) ----------

const state = {
    rotorTypes: ['I', 'II', 'III'],
    ringSettings: ['A', 'A', 'A'],
    startPositions: ['A', 'A', 'A'],   // Grundstellung
    plugPairs: [],                     // e.g. [['A','B'], ...]
    message: ''                        // plaintext typed so far
};

let requestSeq = 0;
let lampTimer = null;
let pendingSocket = null;

// ---------- Build rotor panel ----------

const rotorsEl = document.getElementById('rotors');
const rotorUnits = [];

['Left · slow', 'Middle', 'Right · fast'].forEach((label, i) => {
    const unit = document.createElement('div');
    unit.className = 'rotor';

    const selects = document.createElement('div');
    selects.className = 'rotor-selects';

    const typeSel = document.createElement('select');
    typeSel.title = 'Walzenlage (rotor type)';
    ROTOR_TYPES.forEach(t => typeSel.add(new Option(t, t)));
    typeSel.value = state.rotorTypes[i];
    typeSel.addEventListener('change', () => {
        state.rotorTypes[i] = typeSel.value;
        newMessage();
    });

    const ringSel = document.createElement('select');
    ringSel.title = 'Ringstellung (ring setting)';
    for (const c of ALPHABET) {
        ringSel.add(new Option(`${c} ${String(ALPHABET.indexOf(c) + 1).padStart(2, '0')}`, c));
    }
    ringSel.value = state.ringSettings[i];
    ringSel.addEventListener('change', () => {
        state.ringSettings[i] = ringSel.value;
        newMessage();
    });

    const caption = (text, el) => {
        const wrap = document.createElement('div');
        wrap.className = 'sel-wrap';
        const cap = document.createElement('span');
        cap.className = 'sel-caption';
        cap.textContent = text;
        wrap.append(cap, el);
        return wrap;
    };
    selects.append(caption('Walze', typeSel), caption('Ring', ringSel));

    const wheel = document.createElement('div');
    wheel.className = 'wheel';

    const up = document.createElement('button');
    up.className = 'wheel-btn';
    up.textContent = '▲';
    up.title = 'Turn rotor up';

    const windowEl = document.createElement('div');
    windowEl.className = 'wheel-window';
    const letterEl = document.createElement('div');
    letterEl.className = 'wheel-letter';
    letterEl.textContent = 'A';
    windowEl.appendChild(letterEl);

    const down = document.createElement('button');
    down.className = 'wheel-btn';
    down.textContent = '▼';
    down.title = 'Turn rotor down';

    const turn = delta => {
        const idx = (ALPHABET.indexOf(state.startPositions[i]) + delta + 26) % 26;
        state.startPositions[i] = ALPHABET[idx];
        newMessage();
    };
    up.addEventListener('click', () => turn(1));
    down.addEventListener('click', () => turn(-1));

    wheel.append(up, windowEl, down);

    const wheelCaption = document.createElement('span');
    wheelCaption.className = 'sel-caption';
    wheelCaption.textContent = 'Grundstellung';

    const labelEl = document.createElement('div');
    labelEl.className = 'rotor-label';
    labelEl.textContent = label;

    unit.append(selects, wheelCaption, wheel, labelEl);
    rotorsEl.appendChild(unit);
    rotorUnits.push({ unit, letterEl });
});

// ---------- Build lampboard and keyboard ----------

const lampEls = {};
const keyEls = {};

function buildBoard(containerId, cssClass, store, onPress) {
    const container = document.getElementById(containerId);
    BOARD_ROWS.forEach(row => {
        const rowEl = document.createElement('div');
        rowEl.className = 'board-row';
        row.forEach(letter => {
            const el = document.createElement(onPress ? 'button' : 'div');
            el.className = cssClass;
            el.textContent = letter;
            if (onPress) el.addEventListener('click', () => onPress(letter));
            store[letter] = el;
            rowEl.appendChild(el);
        });
        container.appendChild(rowEl);
    });
}

buildBoard('lampboard', 'lamp', lampEls);
buildBoard('keyboard', 'key', keyEls, letter => typeCharacter(letter));

// ---------- Build plugboard ----------

const plugboardEl = document.getElementById('plugboard');
const cableLayer = document.getElementById('cableLayer');
const socketEls = {};

BOARD_ROWS.forEach(row => {
    const rowEl = document.createElement('div');
    rowEl.className = 'board-row';
    row.forEach(letter => {
        const socket = document.createElement('div');
        socket.className = 'socket';
        socket.innerHTML = `
            <span class="socket-letter">${letter}</span>
            <span class="socket-jacks"><span class="jack"></span><span class="jack"></span></span>`;
        socket.addEventListener('click', () => onSocketClick(letter));
        socketEls[letter] = socket;
        rowEl.appendChild(socket);
    });
    plugboardEl.appendChild(rowEl);
});

function pairOf(letter) {
    return state.plugPairs.find(p => p.includes(letter));
}

function onSocketClick(letter) {
    const existing = pairOf(letter);
    if (existing) {
        state.plugPairs = state.plugPairs.filter(p => p !== existing);
        pendingSocket = null;
        plugboardChanged();
        return;
    }
    if (pendingSocket === letter) {
        pendingSocket = null;
        renderPlugboard();
        return;
    }
    if (pendingSocket === null) {
        if (state.plugPairs.length >= MAX_CABLES) return;
        pendingSocket = letter;
        renderPlugboard();
        return;
    }
    state.plugPairs.push([pendingSocket, letter]);
    pendingSocket = null;
    plugboardChanged();
}

function plugboardChanged() {
    renderPlugboard();
    // Plugboard is part of the daily key: re-encode the current message with it
    processMessage();
}

function renderPlugboard() {
    for (const letter of Object.keys(socketEls)) {
        const el = socketEls[letter];
        el.classList.toggle('pending', pendingSocket === letter);
        const pair = pairOf(letter);
        el.classList.toggle('connected', !!pair);
        if (pair) {
            el.style.setProperty('--plug-color', CABLE_COLORS[state.plugPairs.indexOf(pair) % CABLE_COLORS.length]);
        } else {
            el.style.removeProperty('--plug-color');
        }
    }
    drawCables();
}

function drawCables() {
    const boardRect = plugboardEl.getBoundingClientRect();
    cableLayer.setAttribute('viewBox', `0 0 ${boardRect.width} ${boardRect.height}`);
    cableLayer.innerHTML = '';

    state.plugPairs.forEach((pair, idx) => {
        const color = CABLE_COLORS[idx % CABLE_COLORS.length];
        const [a, b] = pair.map(letter => {
            const r = socketEls[letter].getBoundingClientRect();
            return {
                x: r.left - boardRect.left + r.width / 2,
                y: r.top - boardRect.top + r.height - 8
            };
        });
        const sag = 18 + Math.abs(a.x - b.x) * 0.12;
        const path = document.createElementNS('http://www.w3.org/2000/svg', 'path');
        path.setAttribute('d',
            `M ${a.x} ${a.y} C ${a.x} ${a.y + sag}, ${b.x} ${b.y + sag}, ${b.x} ${b.y}`);
        path.setAttribute('fill', 'none');
        path.setAttribute('stroke', color);
        path.setAttribute('stroke-width', '4');
        path.setAttribute('stroke-linecap', 'round');
        path.setAttribute('opacity', '0.9');
        cableLayer.appendChild(path);
    });
}

window.addEventListener('resize', drawCables);

// ---------- Typing ----------

function typeCharacter(ch) {
    const letter = ch.toUpperCase();
    const isLetter = /^[A-Z]$/.test(letter);
    if (!isLetter && ch !== ' ' && !ALLOWED_PUNCTUATION.includes(ch)) return;

    state.message += isLetter ? letter : ch;

    if (isLetter && keyEls[letter]) {
        keyEls[letter].classList.add('pressed');
        setTimeout(() => keyEls[letter].classList.remove('pressed'), 140);
    }
    processMessage();
}

document.addEventListener('keydown', e => {
    if (e.ctrlKey || e.metaKey || e.altKey) return;
    const target = e.target;
    if (target && (target.tagName === 'SELECT' || target.tagName === 'INPUT' || target.tagName === 'TEXTAREA')) return;

    if (e.key === 'Backspace') {
        e.preventDefault();
        state.message = state.message.slice(0, -1);
        processMessage();
        return;
    }
    if (e.key.length === 1) {
        typeCharacter(e.key);
    }
});

// ---------- API ----------

async function processMessage() {
    const seq = ++requestSeq;
    renderPlainTape();

    if (state.message === '') {
        renderResult(null);
        return;
    }

    try {
        const res = await fetch(`${API_BASE_URL}/api/enigma/process`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                inputText: state.message,
                configuration: {
                    rotors: state.rotorTypes,
                    ringSettings: state.ringSettings.join(''),
                    initialPositions: state.startPositions.join(''),
                    plugboardPairs: state.plugPairs.map(p => p.join(''))
                }
            })
        });
        if (seq !== requestSeq) return; // a newer keystroke superseded this request
        if (!res.ok) {
            setApiStatus(false);
            renderResult(null);
            return;
        }
        const data = await res.json();
        setApiStatus(true);
        renderResult(data);
    } catch {
        if (seq === requestSeq) {
            setApiStatus(false);
            renderResult(null);
        }
    }
}

// ---------- Rendering ----------

const plainTape = document.getElementById('plainTape');
const cipherTape = document.getElementById('cipherTape');

function renderResult(data) {
    const steps = data ? data.steps : [];
    const last = steps.length ? steps[steps.length - 1] : null;

    // Rotor windows: API reports positions rightmost-first; display left-to-right
    const positions = last
        ? [...last.rotorPositions].reverse()
        : [...state.startPositions];
    rotorUnits.forEach((r, i) => {
        if (r.letterEl.textContent !== positions[i]) {
            r.unit.classList.remove('stepped');
            void r.unit.offsetWidth; // restart the tick animation
            r.unit.classList.add('stepped');
        }
        r.letterEl.textContent = positions[i];
    });

    // Lamp: light the output of the last letter briefly
    clearTimeout(lampTimer);
    Object.values(lampEls).forEach(l => l.classList.remove('lit'));
    if (last && lampEls[last.output]) {
        lampEls[last.output].classList.add('lit');
        lampTimer = setTimeout(
            () => lampEls[last.output].classList.remove('lit'), 900);
    }

    // Paper tapes: plaintext as typed, ciphertext in groups of five
    renderPlainTape();

    const cipher = data ? data.cipherText.replace(/\s+/g, '') : '';
    cipherTape.textContent = cipher.replace(/(.{5})/g, '$1 ').trimEnd() || ' ';
}

function renderPlainTape() {
    plainTape.innerHTML = '';
    plainTape.append(state.message, cursorEl());
}

function cursorEl() {
    const c = document.createElement('span');
    c.className = 'cursor';
    c.innerHTML = '&nbsp;';
    return c;
}

function newMessage() {
    // A settings change starts a fresh message with the new key settings
    state.message = '';
    renderResult(null);
}

// ---------- Status / actions ----------

const apiStatusEl = document.getElementById('apiStatus');

function setApiStatus(online) {
    apiStatusEl.classList.toggle('online', online);
    apiStatusEl.classList.toggle('offline', !online);
    apiStatusEl.querySelector('.status-text').textContent = online ? 'online' : 'API offline';
    apiStatusEl.title = online
        ? `Connected to ${API_BASE_URL}`
        : `Cannot reach ${API_BASE_URL} — start EnigmaMachine.Api`;
}

document.getElementById('clearBtn').addEventListener('click', newMessage);

document.getElementById('copyBtn').addEventListener('click', async () => {
    const text = cipherTape.textContent.trim();
    if (!text || text === ' ') return;
    try {
        await navigator.clipboard.writeText(text);
    } catch { /* clipboard unavailable (e.g. non-secure context) */ }
});

// ---------- Init ----------

renderPlugboard();
renderResult(null);

// Probe the API once so the status light is meaningful before the first keystroke
fetch(`${API_BASE_URL}/`)
    .then(res => setApiStatus(res.ok))
    .catch(() => setApiStatus(false));
