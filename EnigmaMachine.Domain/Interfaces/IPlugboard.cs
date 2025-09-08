using System.Collections.Generic;
using EnigmaMachine.Domain.ValueObjects;

namespace EnigmaMachine.Domain.Interfaces
{
    /// <summary>
    /// Defines the interface for the Plugboard component of the Enigma machine.
    /// </summary>
    public interface IPlugboard
    {
        /// <summary>
        /// Connects a pair of letters in the plugboard.
        /// </summary>
        /// <param name="pair">The pair of letters to connect.</param>
        void Connect(PlugboardPair pair);

        /// <summary>
        /// Disconnects a pair of letters in the plugboard.
        /// </summary>
        /// <param name="pair">The pair of letters to disconnect.</param>
        void Disconnect(PlugboardPair pair);

        /// <summary>
        /// Transforms a letter based on the current plugboard connections.
        /// </summary>
        /// <param name="letter">The letter to transform.</param>
        /// <returns>The transformed letter.</returns>
        Letter Transform(Letter letter);

        /// <summary>
        /// Gets all current connections in the plugboard.
        /// </summary>
        /// <returns>A list of connected pairs.</returns>
        IEnumerable<PlugboardPair> GetConnections();
    }
}