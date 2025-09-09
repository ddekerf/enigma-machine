using System;
using System.Collections.Generic;
using EnigmaMachine.Domain.Factories;
using EnigmaMachine.Domain.Interfaces;
using EnigmaMachine.Domain.ValueObjects;
using Xunit;
using EnigmaMachine.Domain.Entities;

namespace EnigmaMachine.Domain.Tests
{
    public class RotorTests
    {
        [Fact]
        public void EnigmaMachineCtor_Throws_When_Rotors_List_Is_Empty()
        {
            var plugboard = new Plugboard();
            var reflector = new ReflectorB();
            var ex = Assert.Throws<ArgumentException>(() => { var _ = new global::EnigmaMachine.Domain.Entities.EnigmaMachine(new List<IRotor>(), plugboard, reflector); });
            Assert.Contains("At least one rotor is required", ex.Message);
        }

        [Fact]
        public void EnigmaMachineCtor_Throws_When_Rotors_List_Contains_Null()
        {
            var plugboard = new Plugboard();
            var reflector = new ReflectorB();
            var rotors = new List<IRotor> { null! };
            Assert.Throws<ArgumentException>(() => { var _ = new global::EnigmaMachine.Domain.Entities.EnigmaMachine(rotors, plugboard, reflector); });
        }
        private static IRotor Create(RotorType type, char ring, char pos)
            => RotorFactory.Create(type, ring, pos);

        [Theory]
        [InlineData(RotorType.I)]
        [InlineData(RotorType.II)]
        [InlineData(RotorType.III)]
        [InlineData(RotorType.IV)]
        [InlineData(RotorType.V)]
        public void ForwardAndBackward_AreInverses(RotorType type)
        {
            var rotor = Create(type, 'A', 'A');
            for (char c = 'A'; c <= 'Z'; c++)
            {
                var f = rotor.ProcessLetter(c);
                var b = rotor.ProcessBackward(f);
                Assert.Equal(c, b);
            }
        }

        [Theory]
        [InlineData(RotorType.I, 'Q')]
        [InlineData(RotorType.II, 'E')]
        [InlineData(RotorType.III, 'V')]
        [InlineData(RotorType.IV, 'J')]
        [InlineData(RotorType.V, 'Z')]
        public void IsAtNotch_TrueAtNotch_AndCycles(RotorType type, char notch)
        {
            var rotor = Create(type, 'A', notch);
            Assert.True(rotor.IsAtNotch);
            rotor.Rotate();
            Assert.False(rotor.IsAtNotch);
            // rotate 25 more times to wrap around to the notch again
            for (int i = 0; i < 25; i++) rotor.Rotate();
            Assert.True(rotor.IsAtNotch);
        }
    }
}
