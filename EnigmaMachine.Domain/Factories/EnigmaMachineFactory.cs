using System;
using System.Collections.Generic;
using EnigmaMachine.Domain.Entities;
using EnigmaMachine.Domain.Interfaces;
using EnigmaMachine.Domain.ValueObjects;

namespace EnigmaMachine.Domain.Factories
{
    /// <summary>
    /// Factory for constructing configured Enigma machines.
    /// </summary>
    public static class EnigmaMachineFactory
    {
        /// <summary>
        /// Creates an Enigma I machine using the provided rotor configuration.
        /// </summary>
        /// <param name="types">Rotor types from right to left.</param>
        /// <param name="ringSettings">Ring settings for each rotor.</param>
        /// <param name="initialPositions">Initial positions for each rotor.</param>
        /// <param name="plugboard">Plugboard configuration.</param>
        /// <param name="reflector">Reflector to use.</param>
        /// <returns>Configured <see cref="EnigmaMachine"/> instance.</returns>
    public static global::EnigmaMachine.Domain.Entities.EnigmaMachine CreateEnigmaI(
            RotorType[] types,
            char[] ringSettings,
            char[] initialPositions,
            IPlugboard plugboard,
            IReflector reflector)
        {
            if (types.Length != 3 || ringSettings.Length != 3 || initialPositions.Length != 3)
            {
                throw new ArgumentException("Enigma I requires exactly three rotors");
            }

            var rotors = new List<IRotor>
            {
                RotorFactory.Create(types[0], ringSettings[0], initialPositions[0]),
                RotorFactory.Create(types[1], ringSettings[1], initialPositions[1]),
                RotorFactory.Create(types[2], ringSettings[2], initialPositions[2])
            };

            return new global::EnigmaMachine.Domain.Entities.EnigmaMachine(rotors, plugboard, reflector);
        }
    }
}
