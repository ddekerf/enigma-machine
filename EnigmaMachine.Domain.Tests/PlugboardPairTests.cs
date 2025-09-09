using EnigmaMachine.Domain.ValueObjects;
using Xunit;

namespace EnigmaMachine.Domain.Tests
{
    public class PlugboardPairTests
    {
        [Fact]
        public void Equality_IsOrderInsensitive()
        {
            var pair1 = new PlugboardPair('A', 'B');
            var pair2 = new PlugboardPair('B', 'A');

            Assert.Equal(pair1, pair2);
            Assert.Equal(pair1.GetHashCode(), pair2.GetHashCode());
        }
    }
}
