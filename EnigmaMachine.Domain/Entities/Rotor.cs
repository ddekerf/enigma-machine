// Rotor.cs
using System;
using EnigmaMachine.Domain.Interfaces;

namespace EnigmaMachine.Domain.Entities
{
    /// <summary>
    /// Represents a rotor in the Enigma machine.
    /// Stores wiring, ring setting, turnover notch and current position.
    /// </summary>
    public sealed class Rotor : IRotor
    {
        private readonly int[] _forwardMapping;
        private readonly int[] _reverseMapping;
        private readonly int _notch;
        private readonly int _ringSetting;
        private int _position;

        /// <summary>
        /// Initializes a new instance of the <see cref="Rotor"/> class.
        /// </summary>
        /// <param name="wiring">The wiring string describing rotor connections.</param>
        /// <param name="notch">The turnover notch position.</param>
        /// <param name="ringSetting">The ring setting character (Ringstellung).</param>
        /// <param name="initialPosition">The initial rotor position character.</param>
        public Rotor(string wiring, char notch, char ringSetting, char initialPosition)
        {
            if (string.IsNullOrWhiteSpace(wiring) || wiring.Length != 26)
                throw new ArgumentException("Wiring must be 26 characters long", nameof(wiring));

            wiring = wiring.ToUpperInvariant();

            // Ensure wiring contains only A-Z and all letters are unique
            var seen = new bool[26];
            for (int i = 0; i < 26; i++)
            {
                char ch = wiring[i];
                if (ch < 'A' || ch > 'Z')
                    throw new ArgumentException("Wiring must contain only A-Z letters", nameof(wiring));
                int idx = ch - 'A';
                if (seen[idx])
                    throw new ArgumentException("Wiring must map each letter exactly once (no duplicates)", nameof(wiring));
                seen[idx] = true;
            }

            _forwardMapping = new int[26];
            _reverseMapping = new int[26];
            for (int i = 0; i < 26; i++)
            {
                int mapped = wiring[i] - 'A';
                _forwardMapping[i] = mapped;
                _reverseMapping[mapped] = i;
            }

            // Validate inputs are uppercase letters
            if (notch < 'A' || notch > 'Z') throw new ArgumentOutOfRangeException(nameof(notch));
            if (ringSetting < 'A' || ringSetting > 'Z') throw new ArgumentOutOfRangeException(nameof(ringSetting));
            if (initialPosition < 'A' || initialPosition > 'Z') throw new ArgumentOutOfRangeException(nameof(initialPosition));

            _notch = notch - 'A';
            _ringSetting = ringSetting - 'A';
            _position = initialPosition - 'A';
        }

        /// <inheritdoc />
        public int Position => _position;

        /// <inheritdoc />
        public bool IsAtNotch => _position == _notch;

        /// <inheritdoc />
        public void Rotate()
        {
            _position = (_position + 1) % 26;
        }

        /// <inheritdoc />
        public char ProcessLetter(char input)
        {
            int c = input - 'A';
            if ((uint)c > 25) throw new ArgumentOutOfRangeException(nameof(input), "Expected uppercase A-Z");

            int s = (_position - _ringSetting + 26) % 26; // netto verschuiving
            int stepped = (c + s) % 26;
            int mapped = _forwardMapping[stepped];
            int result = (mapped - s + 26) % 26;
            return (char)(result + 'A');
        }

        public char ProcessBackward(char input)
        {
            int c = input - 'A';
            if ((uint)c > 25) throw new ArgumentOutOfRangeException(nameof(input), "Expected uppercase A-Z");

            int s = (_position - _ringSetting + 26) % 26;
            int stepped = (c + s) % 26;
            int mapped = _reverseMapping[stepped];
            int result = (mapped - s + 26) % 26;
            return (char)(result + 'A');
        }
    }
}
