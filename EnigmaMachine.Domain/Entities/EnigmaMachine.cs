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
    /// <remarks>
    /// Thread-safety: This type is stateful (rotor positions change per processed letter)
    /// and is not thread-safe. Use a separate instance per concurrent operation
    /// or provide external synchronization. To duplicate state, construct a new instance
    /// with the same configuration and positions.
    /// </remarks>
    public sealed class EnigmaMachine : IEnigmaMachine, IDiagnosticEnigmaMachine
    {
        private readonly IRotor[] _rotors;
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
            if (rotors == null) throw new ArgumentNullException(nameof(rotors));
            _rotors = rotors.ToArray(); // defensive copy to avoid external mutation
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
            for (int i = _rotors.Length - 1; i >= 0; i--)
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
            if (_rotors.Length == 0)
            {
                return;
            }

            // Capture notch state before any rotation (required for double-stepping)
            var atNotchBefore = new bool[_rotors.Length];
            for (int i = 0; i < _rotors.Length; i++)
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
            for (int i = 1; i < _rotors.Length; i++)
            {
                bool carryFromRight = atNotchBefore[i - 1];
                bool selfDoubleStep = i < _rotors.Length - 1 && atNotchBefore[i];
                if (carryFromRight || selfDoubleStep)
                {
                    _rotors[i].Rotate();
                }
            }
        }

        // IDiagnosticEnigmaMachine
        public IReadOnlyList<int> RotorPositions
        {
            get
            {
                var arr = new int[_rotors.Length];
                for (int i = 0; i < _rotors.Length; i++)
                {
                    arr[i] = _rotors[i].Position;
                }
                return arr;
            }
        }
    }
}