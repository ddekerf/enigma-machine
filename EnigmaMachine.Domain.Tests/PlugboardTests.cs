using System;
using EnigmaMachine.Domain.Entities;
using EnigmaMachine.Domain.ValueObjects;
using Xunit;
using EnigmaMachine.Domain.Exceptions;

namespace EnigmaMachine.Domain.Tests
{
    public class PlugboardTests
    {
        [Fact]
        public void ConnectingLetterTwiceThrows()
        {
            var plugboard = new Plugboard();
            plugboard.Connect(new PlugboardPair('A', 'B'));

            var ex = Assert.Throws<DomainOperationException>(() => plugboard.Connect(new PlugboardPair('A', 'C')));
            Assert.Contains("already connected", ex.Message);
        }

        [Fact]
        public void ExceedingMaximumPairsThrows()
        {
            var plugboard = new Plugboard();
            var pairs = new[] { "AB", "CD", "EF", "GH", "IJ", "KL", "MN", "OP", "QR", "ST" };
            foreach (var pair in pairs)
            {
                plugboard.Connect(new PlugboardPair(pair[0], pair[1]));
            }

            Assert.Throws<DomainOperationException>(() => plugboard.Connect(new PlugboardPair('U', 'V')));
        }
    }

}
