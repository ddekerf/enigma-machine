const API_BASE_URL = 'http://localhost:5080';

const rotorOptions = ['I', 'II', 'III', 'IV', 'V'];
const rotorSelects = [
    document.getElementById('rotor1'),
    document.getElementById('rotor2'),
    document.getElementById('rotor3')
];

rotorSelects.forEach((sel, idx) => {
    rotorOptions.forEach(o => {
        const opt = document.createElement('option');
        opt.value = o;
        opt.textContent = o;
        sel.appendChild(opt);
    });
    sel.value = rotorOptions[idx];
});

async function process() {
    const config = {
        rotors: rotorSelects.map(s => s.value),
        ringSettings: document.getElementById('ringSettings').value.toUpperCase(),
        initialPositions: document.getElementById('initialPositions').value.toUpperCase(),
        plugboardPairs: document.getElementById('plugboardPairs').value
            .toUpperCase()
            .split(/\s+/)
            .filter(p => p.length === 2)
    };
    const inputText = document.getElementById('inputText').value;
    try {
        const res = await fetch(`${API_BASE_URL}/api/enigma/process`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ inputText, configuration: config })
        });
        if (!res.ok) return;
        const data = await res.json();
        document.getElementById('outputText').value = data.cipherText;
        const tbody = document.querySelector('#stateTable tbody');
        tbody.innerHTML = '';
        data.steps.forEach(step => {
            const tr = document.createElement('tr');
            tr.innerHTML = `<td>${step.input}</td><td>${step.output}</td><td>${step.rotorPositions.join(' ')}</td>`;
            tbody.appendChild(tr);
        });
    } catch (err) {
        console.error('Error processing text', err);
    }
}

document.querySelectorAll('#config select, #config input, #inputText').forEach(el => {
    el.addEventListener('input', process);
});

process();
