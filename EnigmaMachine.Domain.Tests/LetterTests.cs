using EnigmaMachine.Domain.ValueObjects;
using Xunit;

namespace EnigmaMachine.Domain.Tests
{
    public class LetterTests
    {
        [Theory]
        [InlineData(-1, 'Z')]
        [InlineData(0, 'A')]
        [InlineData(25, 'Z')]
        [InlineData(26, 'A')]
        [InlineData(27, 'B')]
        public void RotorPosition_Normalizes_And_Maps_To_Letters(int index, char expected)
        {
            var rp = new RotorPosition(index);
            Assert.Equal(expected, rp.Letter);
        }

        [Fact]
        public void Letters_WithSameCharacter_AreEqual()
        {
            var a1 = new Letter('A');
            var a2 = new Letter('a');

            Assert.Equal(a1, a2);
        }

        [Fact]
        public void Letters_WithDifferentCharacters_AreNotEqual()
        {
            var a = new Letter('A');
            var b = new Letter('B');

            Assert.NotEqual(a, b);
        }
    }
}
