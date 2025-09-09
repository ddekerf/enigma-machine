using EnigmaMachine.Domain.ValueObjects;
using Xunit;

namespace EnigmaMachine.Domain.Tests
{
    public class LetterTests
    {
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
