using System;

// This file defines the PlugboardPair value object, representing a pair of letters connected in the plugboard.

namespace EnigmaMachine.Domain.ValueObjects
{
    /// <summary>
    /// Represents a pair of letters connected in the plugboard.
    /// </summary>
    public class PlugboardPair
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
            FirstLetter = firstLetter;
            SecondLetter = secondLetter;
        }

        /// <summary>
        /// Determines whether the specified <see cref="PlugboardPair"/> is equal to the current <see cref="PlugboardPair"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="PlugboardPair"/>.</param>
        /// <returns>true if the specified <see cref="object"/> is equal to the current <see cref="PlugboardPair"/>; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return obj is PlugboardPair pair &&
                   FirstLetter == pair.FirstLetter &&
                   SecondLetter == pair.SecondLetter;
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