// Reflector.cs
using System;
using EnigmaMachine.Domain.Interfaces;

namespace EnigmaMachine.Domain.Entities
{
    /// <summary>
    /// Concrete Reflector B using historical wiring YRUHQSLDPXNGOKMIEBFZCWVJAT.
    /// This wiring is an involution (mapping is symmetric) and has no self-mappings.
    /// </summary>
    public sealed class ReflectorB : IReflector
    {
        private const string Wiring = "YRUHQSLDPXNGOKMIEBFZCWVJAT";

        public char Reflect(char input)
        {
            if (input < 'A' || input > 'Z')
                throw new ArgumentOutOfRangeException(nameof(input), "Reflector expects uppercase A-Z");

            return Wiring[input - 'A'];
        }
    }
}