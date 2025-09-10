using System;

namespace EnigmaMachine.Domain.Interfaces
{
    /// <summary>
    /// Transforms free-form text to/from Enigma-safe text (A-Z and spaces) using configurable mappings.
    /// </summary>
    public interface ITextTransformer
    {
        /// <summary>
        /// Encodes input by mapping punctuation/symbols to letter tokens and uppercasing letters.
        /// Output contains only A-Z and whitespace (configurable for unmapped chars).
        /// </summary>
        string Encode(string input);

        /// <summary>
        /// Decodes tokens back to punctuation/symbol text. This can be lossy/ambiguous depending on mapping.
        /// Letters and whitespace are preserved when no token match is found.
        /// </summary>
        string Decode(string input);
    }
}
