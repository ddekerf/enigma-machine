// IRotor.cs
using System;

namespace EnigmaMachine.Domain.Interfaces
{
    /// <summary>
    /// Defines the behavior of a rotor in the Enigma machine.
    /// </summary>
    public interface IRotor
    {
        /// <summary>
        /// Gets the current position of the rotor.
        /// </summary>
        int Position { get; }

        /// <summary>
        /// Rotates the rotor to the next position.
        /// </summary>
        void Rotate();

        /// <summary>
        /// Processes the input letter through the rotor.
        /// </summary>
        /// <param name="input">The input letter to be processed.</param>
        /// <returns>The output letter after processing.</returns>
        char ProcessLetter(char input);
    }
}