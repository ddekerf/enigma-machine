// Rotor.cs
using System;
using EnigmaMachine.Domain.Interfaces;

namespace EnigmaMachine.Domain.Entities
{
    /// <summary>
    /// Represents a rotor in the Enigma machine.
    /// Stores wiring, ring setting, turnover notch and current position.
    /// </summary>
    public class Rotor : IRotor
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
            if (wiring == null || wiring.Length != 26)
            {
                throw new ArgumentException("Wiring must be 26 characters long", nameof(wiring));
            }

            _forwardMapping = new int[26];
            _reverseMapping = new int[26];
            for (int i = 0; i < 26; i++)
            {
                int mapped = wiring[i] - 'A';
                _forwardMapping[i] = mapped;
                _reverseMapping[mapped] = i;
            }

            _notch = notch - 'A';
            _ringSetting = ringSetting - 'A';
            _position = initialPosition - 'A';
        }

        /// <inheritdoc />
        public int Position => _position;

        /// <inheritdoc />
    public bool AtNotch => _position == _notch;

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
            // Convert to 0-25 range
            int c = input - 'A';

            // Adjust for rotor position and ring setting
            int stepped = (c + _position - _ringSetting + 26) % 26;
            int mapped = _forwardMapping[stepped];
            int result = (mapped - _position + _ringSetting + 26) % 26;
            return (char)(result + 'A');
        }

        /// <inheritdoc />
        public char ProcessBackward(char input)
        {
            int c = input - 'A';
            int stepped = (c + _position - _ringSetting + 26) % 26;
            int mapped = _reverseMapping[stepped];
            int result = (mapped - _position + _ringSetting + 26) % 26;
            return (char)(result + 'A');
        }
    }
}
