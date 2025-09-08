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
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            // Initial plugboard transformation
            var signal = _plugboard.Transform(input).Character;

            // Forward pass through the rotors
            foreach (var rotor in _rotors)
            {
                signal = rotor.ProcessLetter(signal);
            }

            // Reflect the signal
            signal = _reflector.Reflect(signal);

            // Reverse pass through the rotors
            for (int i = _rotors.Count - 1; i >= 0; i--)
            {
                signal = _rotors[i].ProcessLetter(signal);
            }

            // Final plugboard transformation
            var output = _plugboard.Transform(new Letter(signal));

            // Rotate rotors for the next letter
            StepRotors();

            return output;
        }

        private void StepRotors()
        {
            if (_rotors.Count == 0)
            {
                return;
            }

            // Check the notch positions before any rotation occurs.
            var fastAtNotchBefore = _rotors[0].IsAtNotch;
            var middleAtNotchBefore = _rotors.Count > 1 && _rotors[1].IsAtNotch;

            // The fast (right-most) rotor always rotates.
            _rotors[0].Rotate();
            _ = _rotors[0].IsAtNotch; // Check notch after rotation

            // The middle rotor rotates if the fast rotor was at its notch or if it
            // is itself at the notch (the double-stepping mechanism).
            if (_rotors.Count > 1 && (fastAtNotchBefore || middleAtNotchBefore))
            {
                _rotors[1].Rotate();
            }
            if (_rotors.Count > 1)
            {
                _ = _rotors[1].IsAtNotch; // Check notch after rotation
            }

            // The left-most rotor rotates only when the middle rotor was at its notch
            // before any rotation took place.
            if (_rotors.Count > 2 && middleAtNotchBefore)
            {
                _rotors[2].Rotate();
            }
        }

        // Additional methods for managing the state of the machine can be added here.
    }
}