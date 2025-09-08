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
    public class EnigmaMachine : IEnigmaMachine
    {
        private readonly List<IRotor> _rotors;
        private readonly IPlugboard _plugboard;
        private readonly IReflector _reflector;

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

            // Step rotors before processing (Enigma stepping occurs prior to encoding)
            StepRotors();

            // Initial plugboard transformation
            var signal = _plugboard.Transform(input).Character;

            // Forward pass through the rotors (right to left)
            foreach (var rotor in _rotors)
            {
                signal = rotor.ProcessLetter(signal);
            }

            // Reflect the signal
            signal = _reflector.Reflect(signal);

            // Reverse pass through the rotors (left to right)
            for (int i = _rotors.Count - 1; i >= 0; i--)
            {
                signal = _rotors[i].ProcessBackward(signal);
            }

            // Final plugboard transformation
            var output = _plugboard.Transform(new Letter(signal));

            return output;
        }

    // IEnigmaMachine intentionally exposes only ProcessLetter

        private void StepRotors()
        {
            if (_rotors.Count == 0)
            {
                return;
            }

            // Capture notch state before any rotation (required for double-stepping)
            var atNotchBefore = new bool[_rotors.Count];
            for (int i = 0; i < _rotors.Count; i++)
            {
                atNotchBefore[i] = _rotors[i].IsAtNotch;
            }

            // Right-most (fast) rotor always rotates
            _rotors[0].Rotate();

            // Apply historical stepping rules:
            // - The middle rotor rotates if the right rotor was at notch (carry)
            //   OR if the middle rotor itself was at notch (double-step scenario).
            // - The leftmost rotor rotates ONLY if the middle rotor was at notch (carry).
            // - For more than 3 rotors, generalize: rotor i rotates if rotor i-1 was at notch;
            //   additionally, a rotor rotates if it itself was at notch, except for the last (leftmost) rotor.
            for (int i = 1; i < _rotors.Count; i++)
            {
                bool carryFromRight = atNotchBefore[i - 1];
                bool selfDoubleStep = i < _rotors.Count - 1 && atNotchBefore[i];
                if (carryFromRight || selfDoubleStep)
                {
                    _rotors[i].Rotate();
                }
            }
        }

        // Additional methods for managing the state of the machine can be added here.
    }
}