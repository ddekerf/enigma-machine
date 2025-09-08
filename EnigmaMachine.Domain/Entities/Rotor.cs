// Rotor.cs
using System;
using EnigmaMachine.Domain.Interfaces;
using EnigmaMachine.Domain.ValueObjects;

namespace EnigmaMachine.Domain.Entities
{
    /// <summary>
    /// Represents a rotor in the Enigma machine.
    /// Supports rotation and signal processing.
    /// </summary>
    public class Rotor : IRotor
    {
        private RotorPosition _position;

        /// <summary>
        /// Initializes a new instance of the <see cref="Rotor"/> class.
        /// </summary>
        /// <param name="initialPosition">The initial rotor position.</param>
        public Rotor(RotorPosition initialPosition)
        {
            _position = initialPosition ?? throw new ArgumentNullException(nameof(initialPosition));
        }

        /// <inheritdoc />
        public int Position => _position.Position;

        /// <inheritdoc />
        public void Rotate()
        {
            _position.Advance();
        }

        /// <inheritdoc />
        public char ProcessLetter(char input)
        {
            // Placeholder processing logic
            return input;
        }
    }
}