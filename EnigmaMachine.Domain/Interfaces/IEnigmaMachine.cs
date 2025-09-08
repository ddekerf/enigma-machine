// IEnigmaMachine.cs
using System;
using System.Collections.Generic;
using EnigmaMachine.Domain.ValueObjects;

namespace EnigmaMachine.Domain.Interfaces
{
    /// <summary>
    /// Defines the interface for the Enigma machine, abstracting its behavior.
    /// </summary>
    public interface IEnigmaMachine
    {
        /// <summary>
        /// Encrypts the given letter using the Enigma machine's configuration.
        /// </summary>
        /// <param name="input">The letter to encrypt.</param>
        /// <returns>The encrypted letter.</returns>
        Letter Encrypt(Letter input);

        /// <summary>
        /// Configures the rotors of the Enigma machine.
        /// </summary>
        /// <param name="rotors">An array of rotors to configure.</param>
        void ConfigureRotors(IRotor[] rotors);

        /// <summary>
        /// Configures the plugboard of the Enigma machine.
        /// </summary>
        /// <param name="plugboard">The plugboard to configure.</param>
        void ConfigurePlugboard(IPlugboard plugboard);

        /// <summary>
        /// Resets the Enigma machine to its initial state.
        /// </summary>
        void Reset();
    }
}