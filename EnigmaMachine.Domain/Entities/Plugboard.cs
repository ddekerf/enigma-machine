using System;
using System.Collections.Generic;
using EnigmaMachine.Domain.Interfaces;
using EnigmaMachine.Domain.ValueObjects;

namespace EnigmaMachine.Domain.Entities
{
    /// <summary>
    /// Represents the plugboard component of the Enigma machine.
    /// </summary>
    public class Plugboard : IPlugboard
    {
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