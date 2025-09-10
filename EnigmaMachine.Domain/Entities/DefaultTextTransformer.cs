using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnigmaMachine.Domain.Interfaces;
using EnigmaMachine.Domain.Exceptions;

namespace EnigmaMachine.Domain.Entities
{
    /// <summary>
    /// Default implementation of ITextTransformer with configurable token mappings.
    /// Follows SRP: only responsible for text tokenization/de-tokenization.
    /// Open/Closed: consumers can provide custom mappings via constructor or alternate implementation.
    /// </summary>
    public sealed class DefaultTextTransformer : ITextTransformer
    {
    private readonly IReadOnlyDictionary<char, string> _punctToToken;
    private readonly IReadOnlyDictionary<string, string> _tokenToText;
        private readonly bool _dropUnknown;

        /// <summary>
        /// Creates a transformer with optional custom mappings.
        /// </summary>
        /// <param name="punctuationToToken">Map of punctuation char to A-Z token (e.g., '.' -> "X"). Tokens must be non-empty and letters only.</param>
        /// <param name="dropUnknown">If true, characters not letters/whitespace and not in mapping are dropped; otherwise preserved as-is (not Enigma-safe).</param>
        public DefaultTextTransformer(
            IDictionary<char, string>? punctuationToToken = null,
            bool dropUnknown = true)
        {
            _punctToToken = (punctuationToToken ?? CreateDefaultMapping())
                .ToDictionary(kv => kv.Key, kv => ValidateToken(kv.Value));
            // Build reverse mapping with sensible defaults for duplicate tokens.
            var reverse = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var kv in _punctToToken)
            {
                var token = kv.Value;
                var text = kv.Key.ToString();
                if (!reverse.ContainsKey(token))
                {
                    reverse[token] = text;
                }
            }
            // Normalize duplicates to preferred representations
            if (reverse.ContainsKey("YY")) reverse["YY"] = "/"; // prefer slash for slant/dash/hyphen
            if (reverse.ContainsKey("KK")) reverse["KK"] = "()"; // represent parentheses pair
            _tokenToText = reverse;
            _dropUnknown = dropUnknown;
        }

        public string Encode(string input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            var sb = new StringBuilder(input.Length * 2);
            foreach (var ch in input)
            {
                if (char.IsLetter(ch))
                {
                    sb.Append(char.ToUpperInvariant(ch));
                }
                else if (char.IsWhiteSpace(ch))
                {
                    sb.Append(ch);
                }
                else if (_punctToToken.TryGetValue(ch, out var token))
                {
                    sb.Append(token);
                }
                else if (!_dropUnknown)
                {
                    // Preserve as-is (consumer may choose to pre-clean input instead)
                    sb.Append(ch);
                }
                // else: drop unknown chars
            }
            return sb.ToString();
        }

        public string Decode(string input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            var sb = new StringBuilder(input.Length);

            int i = 0;
            while (i < input.Length)
            {
                char ch = input[i];
                if (!char.IsLetter(ch))
                {
                    sb.Append(ch);
                    i++;
                    continue;
                }

                // Try to match the longest token at this position
                // Tokens are uppercase A-Z sequences; we choose the longest defined token to avoid prefix ambiguity.
        string? resolved = null;
                int matchLen = 0;
        foreach (var kv in _tokenToText.OrderByDescending(k => k.Key.Length))
                {
                    var token = kv.Key;
                    if (i + token.Length <= input.Length && string.CompareOrdinal(input, i, token, 0, token.Length) == 0)
                    {
            resolved = kv.Value;
                        matchLen = token.Length;
                        break;
                    }
                }

        if (resolved != null)
                {
            sb.Append(resolved);
                    i += matchLen;
                }
                else
                {
                    // Not a token, keep the letter as-is
                    sb.Append(ch);
                    i++;
                }
            }

            return sb.ToString();
        }

        private static IDictionary<char, string> CreateDefaultMapping()
        {
            // Requirements:
            // '.' -> "X"
            // ':' -> "XX"
            // '?' -> "UD"
            // ',' -> "Y"
            // '/' and '-' and hyphen -> "YY" (treat slash and dash the same per description)
            // parenthesis -> "KK" (we'll map both '(' and ')' to same token for symmetry)
            return new Dictionary<char, string>
            {
                ['.'] = "X",
                [':'] = "XX",
                ['?'] = "UD",
                [','] = "Y",
                ['/'] = "YY",
                ['-'] = "YY",
                ['('] = "KK",
                [')'] = "KK",
            };
        }

        private static string ValidateToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new DomainValidationException("Token must be non-empty letters.");
            foreach (var c in token)
            {
                if (!char.IsLetter(c))
                    throw new DomainValidationException("Token must contain only letters A-Z.");
            }
            return token.ToUpperInvariant();
        }
    }
}
