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
    }
}
