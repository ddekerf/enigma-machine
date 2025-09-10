using System.Text;
using EnigmaMachine.Domain.Interfaces;
using EnigmaMachine.Domain.ValueObjects;
using EnigmaMachine.Domain.Exceptions;

namespace EnigmaMachine.Domain.Extensions
{
    public static class EnigmaMachineExtensions
    {
        /// <summary>
        /// Processes a string through the Enigma machine. Letters are normalized to uppercase A-Z.
        /// Non-letters are passed through unchanged when passThroughNonLetters is true.
        /// </summary>
        public static string Process(this IEnigmaMachine machine, string input, bool passThroughNonLetters = true)
        {
            if (machine is null) throw new System.ArgumentNullException(nameof(machine));
            if (input is null) throw new System.ArgumentNullException(nameof(input));

            var sb = new StringBuilder(input.Length);
            foreach (var ch in input)
            {
                if (char.IsLetter(ch))
                {
                    var letter = new Letter(ch);
                    sb.Append(machine.ProcessLetter(letter).Character);
                }
                else
                {
                    if (passThroughNonLetters)
                    {
                        sb.Append(ch);
                    }
                    else
                    {
                        throw new DomainValidationException("Input contains non-letter characters and passThroughNonLetters is false.");
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Encodes free-form text by mapping punctuation to tokens, processes through Enigma, then returns the cipher text.
        /// Output will contain only A-Z and whitespace (depending on transformer configuration).
        /// </summary>
        public static string EncodeText(this IEnigmaMachine machine, string input, ITextTransformer? transformer = null)
        {
            if (machine is null) throw new System.ArgumentNullException(nameof(machine));
            if (input is null) throw new System.ArgumentNullException(nameof(input));
            transformer ??= new Entities.DefaultTextTransformer();
            var preprocessed = transformer.Encode(input);
            var sb = new StringBuilder(preprocessed.Length);
            foreach (var ch in preprocessed)
            {
                if (char.IsLetter(ch))
                {
                    var letter = new Letter(ch);
                    sb.Append(machine.ProcessLetter(letter).Character);
                }
                else
                {
                    // preserve whitespace and any transformer-chosen pass-through
                    sb.Append(ch);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Decodes a cipher text by running it back through a synchronized machine instance and decoding tokens back to punctuation.
        /// Callers are responsible for using a machine with identical configuration and starting state as used for encoding.
        /// </summary>
        public static string DecodeText(this IEnigmaMachine machine, string cipher, ITextTransformer? transformer = null)
        {
            if (machine is null) throw new System.ArgumentNullException(nameof(machine));
            if (cipher is null) throw new System.ArgumentNullException(nameof(cipher));
            transformer ??= new Entities.DefaultTextTransformer();
            var sb = new StringBuilder(cipher.Length);
            foreach (var ch in cipher)
            {
                if (char.IsLetter(ch))
                {
                    var letter = new Letter(ch);
                    sb.Append(machine.ProcessLetter(letter).Character);
                }
                else
                {
                    sb.Append(ch);
                }
            }
            var decoded = transformer.Decode(sb.ToString());
            return decoded;
        }
    }
}
