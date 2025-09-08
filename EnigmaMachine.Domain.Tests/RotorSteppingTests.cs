using System.Collections.Generic;
using EnigmaMachine.Domain.Entities;
using EnigmaMachine.Domain.Interfaces;
using EnigmaMachine.Domain.ValueObjects;
using Xunit;

namespace EnigmaMachine.Domain.Tests
{
    public class RotorSteppingTests
    {
        [Fact]
        public void DoubleStepOccursAroundNotchTransitions()
        {
            // Arrange: fast rotor at notch, middle one step before its notch
            var fast = new Rotor(new RotorPosition(25));
            var middle = new Rotor(new RotorPosition(24));
            var left = new Rotor(new RotorPosition(0));
            var machine = new EnigmaMachine(new List<IRotor> { fast, middle, left }, new Plugboard(), new Reflector());

            // Act - first key press
            machine.ProcessLetter(new Letter('A'));

            // Assert intermediate positions
            Assert.Equal(26, fast.Position);
            Assert.Equal(25, middle.Position);
            Assert.Equal(0, left.Position);

            // Act - second key press
            machine.ProcessLetter(new Letter('A'));

            // Assert - middle rotor stepped twice and left rotor stepped once
            Assert.Equal(27, fast.Position);
            Assert.Equal(26, middle.Position);
            Assert.Equal(1, left.Position);
        }
    }
}
