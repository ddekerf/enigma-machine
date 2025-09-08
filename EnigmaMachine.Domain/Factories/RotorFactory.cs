using System.Collections.Generic;
using EnigmaMachine.Domain.Entities;
using EnigmaMachine.Domain.Interfaces;
using EnigmaMachine.Domain.ValueObjects;

namespace EnigmaMachine.Domain.Factories
{
    /// <summary>
    /// Factory for creating rotors with historical presets.
    /// </summary>
    public static class RotorFactory
    {
        private static readonly Dictionary<RotorType, (string Wiring, char Notch)> RotorPresets =
            new()
            {
                { RotorType.I, ("EKMFLGDQVZNTOWYHXUSPAIBRCJ", 'Q') },
                { RotorType.II, ("AJDKSIRUXBLHWTMCQGZNPYFVOE", 'E') },
                { RotorType.III, ("BDFHJLCPRTXVZNYEIWGAKMUSQO", 'V') },
                { RotorType.IV, ("ESOVPZJAYQUIRHXLNFTGKDCMWB", 'J') },
                { RotorType.V, ("VZBRGITYUPSDNHLXAWMJQOFECK", 'Z') }
            };

        /// <summary>
        /// Creates a rotor of the specified type using historical wiring and notch positions.
        /// </summary>
        /// <param name="type">The rotor type.</param>
        /// <param name="ringSetting">The ring setting character.</param>
        /// <param name="initialPosition">The initial rotor position.</param>
        /// <returns>A configured rotor instance.</returns>
        public static IRotor Create(RotorType type, char ringSetting, char initialPosition)
        {
            var config = RotorPresets[type];
            return new Rotor(config.Wiring, config.Notch, ringSetting, initialPosition);
        }
    }
}
