using System;
using System.Collections.Generic;
using EnigmaMachine.Domain.Interfaces;
using EnigmaMachine.Domain.ValueObjects;

namespace EnigmaMachine.Domain.Entities
{
    /// <summary>
    /// Represents the plugboard component of the Enigma machine.
    /// </summary>
    public sealed class Plugboard : IPlugboard
    {
        private const int MaxPairs = 10;
        private readonly Dictionary<char, char> _connections;

        /// <summary>
        /// Initializes a new instance of the <see cref="Plugboard"/> class.
        /// </summary>
        public Plugboard()
        {
            _connections = new Dictionary<char, char>();
        }

        // Keep internal helper
        private void ConnectInternal(char letter1, char letter2)
        {
            if (letter1 == letter2)
                throw new ArgumentException("Cannot connect a letter to itself.");

            if (!char.IsLetter(letter1) || !char.IsLetter(letter2))
                throw new ArgumentException("Only letters A-Z are allowed.");

            letter1 = char.ToUpperInvariant(letter1);
            letter2 = char.ToUpperInvariant(letter2);

            if (_connections.ContainsKey(letter1))
                throw new InvalidOperationException($"Letter '{letter1}' is already connected to '{_connections[letter1]}'.");

            if (_connections.ContainsKey(letter2))
                throw new InvalidOperationException($"Letter '{letter2}' is already connected to '{_connections[letter2]}'.");

            if (_connections.Count >= MaxPairs * 2)
                throw new InvalidOperationException($"Plugboard can have at most {MaxPairs} pairs.");

            _connections[letter1] = letter2;
            _connections[letter2] = letter1;
        }

        private void DisconnectInternal(char letter1, char letter2)
        {
            if (_connections.ContainsKey(letter1) && _connections[letter1] == letter2)
            {
                _connections.Remove(letter1);
                _connections.Remove(letter2);
            }
        }

        private char GetConnectedLetter(char letter)
        {
            if (!char.IsLetter(letter))
                throw new ArgumentException("Only letters A-Z are allowed.");
            letter = char.ToUpperInvariant(letter);
            return _connections.TryGetValue(letter, out var connectedLetter) ? connectedLetter : letter;
        }

        // IPlugboard implementation
        public void Connect(PlugboardPair pair)
        {
            ConnectInternal(pair.FirstLetter, pair.SecondLetter);
        }

        public void Disconnect(PlugboardPair pair)
        {
            DisconnectInternal(pair.FirstLetter, pair.SecondLetter);
        }

        public Letter Transform(Letter letter)
        {
            var transformed = GetConnectedLetter(letter.Character);
            return new Letter(transformed);
        }

        public IEnumerable<PlugboardPair> GetConnections()
        {
            var seen = new HashSet<char>();
            foreach (var kvp in _connections)
            {
                var a = kvp.Key;
                var b = kvp.Value;
                if (!seen.Contains(a) && !seen.Contains(b))
                {
                    seen.Add(a);
                    seen.Add(b);
                    yield return new PlugboardPair(a, b);
                }
            }
        }
    }
}