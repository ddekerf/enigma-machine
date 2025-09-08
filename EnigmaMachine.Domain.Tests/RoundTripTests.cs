using System.Linq;
using EnigmaMachine.Domain.Entities;
using EnigmaMachine.Domain.Extensions;
using EnigmaMachine.Domain.Factories;
using EnigmaMachine.Domain.Interfaces;
using EnigmaMachine.Domain.ValueObjects;
using Xunit;

namespace EnigmaMachine.Domain.Tests
{
    public class RoundTripTests
    {
        private static IEnigmaMachine Build()
        {
            var plug = new Plugboard();
            foreach (var pair in new[] { "AB", "CD", "EF", "GH", "IJ" })
                plug.Connect(new PlugboardPair(pair[0], pair[1]));
            var refl = new ReflectorB();
            var types = new[] { RotorType.I, RotorType.III, RotorType.V };
            var rings = "ABC".ToCharArray().Reverse().ToArray();
            var pos = "XYZ".ToCharArray().Reverse().ToArray();
            return EnigmaMachineFactory.CreateEnigmaI(types.Reverse().ToArray(), rings, pos, plug, refl);
        }

        [Fact]
        public void EncryptThenDecrypt_RoundTrips()
        {
            var m1 = Build();
            var cipher = m1.Process("HELLO HOW ARE YOU?");
            var m2 = Build();
            var plain = m2.Process(cipher);
            Assert.Equal("HELLO HOW ARE YOU?", plain);
        }
    }
}
