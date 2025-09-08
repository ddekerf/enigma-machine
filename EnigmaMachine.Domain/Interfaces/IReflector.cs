// IReflector.cs
using System;

namespace EnigmaMachine.Domain.Interfaces
{
    /// <summary>
    /// Defines the interface for the Reflector component of the Enigma machine.
    /// </summary>
    public interface IReflector
    {
        /// <summary>
        /// Reflects the input signal based on the reflector's wiring.
        /// </summary>
        /// <param name="input">The input signal to reflect.</param>
        /// <returns>The reflected signal.</returns>
        char Reflect(char input);
    }
}
