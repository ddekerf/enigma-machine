// EnigmaMachine.Domain/Entities/EnigmaMachine.cs
using System;
using System.Collections.Generic;
using EnigmaMachine.Domain.Interfaces;
using EnigmaMachine.Domain.ValueObjects;

namespace EnigmaMachine.Domain.Entities
{
    /// <summary>
    /// Represents the Enigma machine as the aggregate root.
    /// </summary>
    public class EnigmaMachine
    {
        private readonly List<IRotor> _rotors;
        private IPlugboard _plugboard;
        private IReflector _reflector;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnigmaMachine"/> class.
        /// </summary>
        /// <param name="rotors">The list of rotors to be used in the machine.</param>
        /// <param name="plugboard">The plugboard component of the machine.</param>
        /// <param name="reflector">The reflector component of the machine.</param>
        public EnigmaMachine(List<IRotor> rotors, IPlugboard plugboard, IReflector reflector)
        {
            _rotors = rotors ?? throw new ArgumentNullException(nameof(rotors));
            _plugboard = plugboard ?? throw new ArgumentNullException(nameof(plugboard));
            _reflector = reflector ?? throw new ArgumentNullException(nameof(reflector));
        }

        /// <summary>
        /// Encrypts or decrypts a letter using the Enigma machine.
        /// </summary>
        /// <param name="input">The input letter to be processed.</param>
        /// <returns>The resulting letter after processing.</returns>
        public Letter ProcessLetter(Letter input)
        {
            // Implementation of letter processing logic goes here.
            // This will involve passing the letter through the plugboard,
            // rotors, and reflector in sequence.

            throw new NotImplementedException();
        }

        // Additional methods for managing the state of the machine can be added here.
    }
}