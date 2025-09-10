using System;
using System.Collections.Generic;
using EnigmaMachine.Domain.Entities;
using EnigmaMachine.Domain.Interfaces;
using EnigmaMachine.Domain.ValueObjects;
using EnigmaMachine.Domain.Exceptions;

namespace EnigmaMachine.Domain.Factories
{
    /// <summary>
    /// Factory for constructing configured Enigma machines.
    /// </summary>
    public static class EnigmaMachineFactory
    {
        /// <summary>
        /// Creates an Enigma I machine using the provided rotor configuration.
        /// Order: arrays are expected RIGHT to LEFT (fast/rightmost first).
        /// </summary>
        /// <param name="types">Rotor types from right to left.</param>
        /// <param name="ringSettings">Ring settings for each rotor.</param>
        /// <param name="initialPositions">Initial positions for each rotor.</param>
        /// <param name="plugboard">Plugboard configuration.</param>
        /// <param name="reflector">Reflector to use.</param>
        /// <returns>Configured <see cref="IEnigmaMachine"/> instance.</returns>
        public static IEnigmaMachine CreateEnigmaI(
                RotorType[] types,
                char[] ringSettings,
                char[] initialPositions,
                IPlugboard plugboard,
                IReflector reflector)
        {
            if (types.Length != 3 || ringSettings.Length != 3 || initialPositions.Length != 3)
            {
                throw new DomainValidationException("Enigma I requires exactly three rotors");
            }

            ValidateAtoZ(ringSettings, nameof(ringSettings));
            ValidateAtoZ(initialPositions, nameof(initialPositions));

            var rotors = new List<IRotor>
            {
                RotorFactory.Create(types[0], ringSettings[0], initialPositions[0]),
                RotorFactory.Create(types[1], ringSettings[1], initialPositions[1]),
                RotorFactory.Create(types[2], ringSettings[2], initialPositions[2])
            };

            return new EnigmaMachine.Domain.Entities.EnigmaMachine(rotors, plugboard, reflector);
        }

        /// <summary>
        /// Creates an Enigma I machine using arrays provided LEFT to RIGHT (historical human order).
        /// </summary>
        /// <remarks>
        /// Order: supply rotors, ring settings, and positions from left (slow) to right (fast),
        /// matching common documentation. This method reverses them internally to the machine's
        /// right-to-left processing order.
        /// Constraints: exactly three items per array (Enigma I) and all characters must be in A–Z.
        /// </remarks>
        /// <param name="typesLeftToRight">Rotor types from left (slow) to right (fast).</param>
        /// <param name="ringSettingsLeftToRight">Ring settings from left to right.</param>
        /// <param name="initialPositionsLeftToRight">Initial positions from left to right.</param>
        /// <param name="plugboard">Plugboard configuration.</param>
        /// <param name="reflector">Reflector to use.</param>
        /// <returns>Configured <see cref="IEnigmaMachine"/> instance.</returns>
        public static IEnigmaMachine CreateEnigmaILeftToRight(
            RotorType[] typesLeftToRight,
            char[] ringSettingsLeftToRight,
            char[] initialPositionsLeftToRight,
            IPlugboard plugboard,
            IReflector reflector)
        {
            if (typesLeftToRight.Length != 3 || ringSettingsLeftToRight.Length != 3 || initialPositionsLeftToRight.Length != 3)
            {
                throw new DomainValidationException("Enigma I requires exactly three rotors");
            }

            ValidateAtoZ(ringSettingsLeftToRight, nameof(ringSettingsLeftToRight));
            ValidateAtoZ(initialPositionsLeftToRight, nameof(initialPositionsLeftToRight));

            // Reverse to RIGHT->LEFT expected by constructor
            var types = (RotorType[])typesLeftToRight.Clone();
            Array.Reverse(types);
            var ringSettings = (char[])ringSettingsLeftToRight.Clone();
            Array.Reverse(ringSettings);
            var initialPositions = (char[])initialPositionsLeftToRight.Clone();
            Array.Reverse(initialPositions);

            return CreateEnigmaI(types, ringSettings, initialPositions, plugboard, reflector);
        }

        private static void ValidateAtoZ(char[] arr, string paramName)
        {
            foreach (var c in arr)
            {
                if (c < 'A' || c > 'Z')
                    throw new ArgumentOutOfRangeException(paramName, "Only A-Z are allowed.");
            }
        }
    }
}
