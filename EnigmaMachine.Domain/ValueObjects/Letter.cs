using System;

// Letter.cs
namespace EnigmaMachine.Domain.ValueObjects
{
    /// <summary>
    /// Represents a single letter in the encryption process.
    /// </summary>
    public readonly record struct Letter
    {
        /// <summary>
        /// Gets the character representing the letter.
        /// </summary>
        public char Character { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Letter"/> record struct.
        /// </summary>
        /// <param name="character">The character representing the letter.</param>
        /// <exception cref="ArgumentException">Thrown when the character is not a valid letter.</exception>
        public Letter(char character)
        {
            if (!char.IsLetter(character))
            {
                throw new ArgumentException("Character must be a letter.", nameof(character));
            }

            Character = char.ToUpperInvariant(character);
        }

        /// <summary>
        /// Returns a string that represents the current letter.
        /// </summary>
        /// <returns>A string that represents the current letter.</returns>
        public override string ToString()
        {
            return Character.ToString();
        }
    }
}
