// IEnigmaMachine.cs
using EnigmaMachine.Domain.ValueObjects;

namespace EnigmaMachine.Domain.Interfaces
{
    /// <summary>
    /// Minimal contract for an Enigma machine: process a single letter with current state.
    /// Configuration is provided externally (e.g., via factory/constructor).
    /// </summary>
    public interface IEnigmaMachine
    {
        /// <summary>
        /// Encrypts/decrypts a single letter, advancing rotor state appropriately.
        /// </summary>
        /// <param name="input">The input letter (A-Z).</param>
        /// <returns>The processed letter.</returns>
        Letter ProcessLetter(Letter input);
    }
}