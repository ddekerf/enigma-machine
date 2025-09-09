using EnigmaMachine.Domain.Entities;
using EnigmaMachine.Domain.Extensions;
using EnigmaMachine.Domain.Factories;
using EnigmaMachine.Domain.Interfaces;
using EnigmaMachine.Domain.ValueObjects;
using Xunit;

namespace EnigmaMachine.Domain.Tests
{
    public class ExtensionsTests
    {
        private static IEnigmaMachine Build()
        {
            var plug = new Plugboard();
            var refl = new ReflectorB();
            var types = new[] { RotorType.I, RotorType.II, RotorType.III };
            var rings = new[] { 'A', 'A', 'A' };
            var pos = new[] { 'A', 'A', 'A' };
            return EnigmaMachineFactory.CreateEnigmaI(types, rings, pos, plug, refl);
        }

        [Fact]
        public void Process_PassesThroughSpacesAndPunctuation()
        {
            var m = Build();
            var output = m.Process("HI THERE!", passThroughNonLetters: true);
            Assert.Equal(9, output.Length);
            Assert.Equal('!', output[^1]);
        }

        [Fact]
        public void Process_Throws_OnNonLetters_WhenDisabled()
        {
            var m = Build();
            Assert.Throws<System.ArgumentException>(() => m.Process("HI!", passThroughNonLetters: false));
        }

        [Fact]
        public void EncodeText_DefaultTransformer_MapsPunctuation()
        {
            var m = Build();
            // Using identity processing (same machine settings for encode/decode), ensure mapping is reversible
            var plain = "HELLO. WHY? A:B, C/D-(E)";
            var cipher = m.EncodeText(plain); // encode via default transformer

            // Reset machine to same initial state for decoding
            m = Build();
            var roundTrip = m.DecodeText(cipher);

            // Decoding uses normalized forms for duplicate tokens: '/' for YY, and "()" for KK
            // Instead, validate via transformer behavior: after encode->decode with same default transformer and no machine,
            // we should get the same normalization as roundTrip when using identity ciphering.
            var t = new EnigmaMachine.Domain.Entities.DefaultTextTransformer();
            var normalized = t.Decode(t.Encode(plain.ToUpperInvariant()));
            Assert.Equal(normalized, roundTrip);
        }

        private sealed class CustomTransformer : EnigmaMachine.Domain.Interfaces.ITextTransformer
        {
            public string Encode(string input)
            {
                // Map '!' to token "QQQ" and otherwise behave like identity uppercase letters
                var sb = new System.Text.StringBuilder();
                foreach (var ch in input)
                {
                    if (char.IsLetter(ch)) sb.Append(char.ToUpperInvariant(ch));
                    else if (ch == '!') sb.Append("QQQ");
                    else if (char.IsWhiteSpace(ch)) sb.Append(ch);
                }
                return sb.ToString();
            }

            public string Decode(string input)
            {
                // Reverse "QQQ" back to '!'
                var s = input.Replace("QQQ", "!");
                return s;
            }
        }

        [Fact]
        public void EncodeDecodeText_CustomTransformer_Works()
        {
            var m = Build();
            var t = new CustomTransformer();
            var plain = "HI!";
            var cipher = m.EncodeText(plain, t);

            m = Build();
            var roundTrip = m.DecodeText(cipher, t);
            Assert.Equal("HI!", roundTrip);
        }
    }
}
