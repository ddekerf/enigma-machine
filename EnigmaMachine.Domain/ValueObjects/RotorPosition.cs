// RotorPosition.cs
namespace EnigmaMachine.Domain.ValueObjects
{
    using System;

    /// <summary>
    /// Represents the position of a rotor in the Enigma machine.
    /// </summary>
    public class RotorPosition
    {
        /// <summary>
        /// Gets the current position of the rotor.
        /// </summary>
        public int Position { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RotorPosition"/> class.
        /// </summary>
        /// <param name="position">The initial position of the rotor.</param>
        public RotorPosition(int position)
        {
            if (position < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(position), "Position cannot be negative.");
            }

            Position = position;
        }

        /// <summary>
        /// Advances the rotor position by one.
        /// </summary>
        public void Advance()
        {
            Position++;
        }

        /// <summary>
        /// Resets the rotor position to the initial state.
        /// </summary>
        public void Reset()
        {
            Position = 0;
        }
    }
}