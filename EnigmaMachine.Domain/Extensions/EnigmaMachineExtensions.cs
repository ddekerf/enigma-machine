using System.Text;
using EnigmaMachine.Domain.Interfaces;
using EnigmaMachine.Domain.ValueObjects;

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
                        throw new System.ArgumentException("Input contains non-letter characters and passThroughNonLetters is false.");
                    }
                }
            }
            return sb.ToString();
        }
    }
}
