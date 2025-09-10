using System;
using EnigmaMachine.Domain.Exceptions;

// This file defines the PlugboardPair value object, representing a pair of letters connected in the plugboard.

namespace EnigmaMachine.Domain.ValueObjects
{
    /// <summary>
    /// Represents a pair of letters connected in the plugboard.
    /// </summary>
    public sealed class PlugboardPair
    {
        /// <summary>
        /// Gets the first letter of the pair.
        /// </summary>
        public char FirstLetter { get; }

        /// <summary>
        /// Gets the second letter of the pair.
        /// </summary>
        public char SecondLetter { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlugboardPair"/> class.
        /// </summary>
        /// <param name="firstLetter">The first letter of the pair.</param>
        /// <param name="secondLetter">The second letter of the pair.</param>
        public PlugboardPair(char firstLetter, char secondLetter)
        {
            if (!char.IsLetter(firstLetter) || !char.IsLetter(secondLetter))
                throw new DomainValidationException("Plugboard pairs must be letters A-Z");

            firstLetter = char.ToUpperInvariant(firstLetter);
            secondLetter = char.ToUpperInvariant(secondLetter);

            if (firstLetter == secondLetter)
                throw new DomainValidationException("Plugboard cannot connect a letter to itself.");

            // Normalize ordering so that (A,B) is considered equal to (B,A)
            if (firstLetter > secondLetter)
            {
                (firstLetter, secondLetter) = (secondLetter, firstLetter);
            }

            FirstLetter = firstLetter;
            SecondLetter = secondLetter;
        }

        /// <summary>
        /// Determines whether the specified <see cref="PlugboardPair"/> is equal to the current <see cref="PlugboardPair"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="PlugboardPair"/>.</param>
        /// <returns>true if the specified <see cref="object"/> is equal to the current <see cref="PlugboardPair"/>; otherwise, false.</returns>
        public override bool Equals(object? obj)
        {
            return obj is PlugboardPair pair && Equals(pair);
        }

        public bool Equals(PlugboardPair? other)
        {
            if (other is null) return false;
            return FirstLetter == other.FirstLetter && SecondLetter == other.SecondLetter;
        }

        /// <summary>
        /// Returns a hash code for the current <see cref="PlugboardPair"/>.
        /// </summary>
        /// <returns>A hash code for the current <see cref="PlugboardPair"/>.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(FirstLetter, SecondLetter);
        }
    }
}