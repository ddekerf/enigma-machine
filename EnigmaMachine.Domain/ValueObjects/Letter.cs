using System;

// Letter.cs
namespace EnigmaMachine.Domain.ValueObjects
{
    /// <summary>
    /// Represents a single letter in the encryption process.
    /// </summary>
    public class Letter
    {
        /// <summary>
        /// Gets the character representing the letter.
        /// </summary>
        public char Character { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Letter"/> class.
        /// </summary>
        /// <param name="character">The character representing the letter.</param>
        /// <exception cref="ArgumentException">Thrown when the character is not a valid letter.</exception>
        public Letter(char character)
        {
            if (!char.IsLetter(character))
            {
                throw new ArgumentException("Character must be a letter.", nameof(character));
            }

            Character = char.ToUpper(character);
        }

        /// <summary>
        /// Returns a string that represents the current letter.
        /// </summary>
        /// <returns>A string that represents the current letter.</returns>
        public override string ToString()
        {
            return Character.ToString();
        }

        // Additional methods and overrides (e.g., Equals, GetHashCode) can be added as needed.
    }
}