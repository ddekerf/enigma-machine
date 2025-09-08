using System.Linq;
using System.Text;
using EnigmaMachine.Domain.Entities;
using EnigmaMachine.Domain.Factories;
using EnigmaMachine.Domain.Interfaces;
using EnigmaMachine.Domain.ValueObjects;
using Xunit;

namespace EnigmaMachine.Domain.Tests
{
    public class EnigmaDecodingTests
    {
        private static IEnigmaMachine BuildMachine(RotorType[] rotorsLeftToRight, string rotorStart, string ringSettings)
        {
            var plugboard = new Plugboard();
            foreach (var pair in "BA QU CG XT DI ER JW LS VK NM".Split(' ', System.StringSplitOptions.RemoveEmptyEntries))
            {
                plugboard.Connect(new PlugboardPair(pair[0], pair[1]));
            }

            IReflector reflector = new ReflectorB();

            var rotorTypes = rotorsLeftToRight.Reverse().ToArray();
            var positions = rotorStart.ToUpper().ToCharArray().Reverse().ToArray();
            var rings = ringSettings.ToUpper().ToCharArray().Reverse().ToArray();

            return EnigmaMachineFactory.CreateEnigmaI(rotorTypes, rings, positions, plugboard, reflector);
        }

        private static string Process(IEnigmaMachine machine, string data)
        {
            var sb = new StringBuilder();
            foreach (var ch in data)
            {
                var letter = new Letter(ch);
                sb.Append(machine.ProcessLetter(letter).Character);
            }
            return sb.ToString();
        }

        [Fact]
        public void Rotors_I_III_V_WWW_BBB_Decrypts_Correctly()
        {
            var machine = TestHelpers.BuildMachine(new[] { RotorType.I, RotorType.III, RotorType.V }, "WWW", "BBB");
            var result = TestHelpers.Process(machine, "DXXIQUIJZMNBEH");
            Assert.Equal("HELLOHOWAREYOU", result);
        }

        [Fact]
        public void Rotors_I_III_V_ABC_XYZ_Decrypts_Correctly()
        {
            var machine = TestHelpers.BuildMachine(new[] { RotorType.I, RotorType.III, RotorType.V }, "ABC", "XYZ");
            var result = TestHelpers.Process(machine, "YNHWFVJEMWGLVY");
            Assert.Equal("HELLOHOWAREYOU", result);
        }

        [Fact]
        public void Rotors_I_III_IV_ABC_XYZ_Decrypts_Correctly()
        {
            var machine = TestHelpers.BuildMachine(new[] { RotorType.I, RotorType.III, RotorType.IV }, "ABC", "XYZ");
            var result = TestHelpers.Process(machine, "PKWSGCUAYAMUDG");
            Assert.Equal("HELLOHOWAREYOU", result);
        }

        [Fact]
        public void Rotors_IV_III_V_ABC_XYZ_Decrypts_Correctly()
        {
            var machine = TestHelpers.BuildMachine(new[] { RotorType.IV, RotorType.III, RotorType.V }, "ABC", "XYZ");
            var result = TestHelpers.Process(machine, "ZMDVVLHGBMXMTN");
            Assert.Equal("HELLOHOWAREYOU", result);
        }

        [Fact]
        public void Rotors_I_II_III_AAA_ZZZ_Decrypts_Correctly()
        {
            var machine = TestHelpers.BuildMachine(new[] { RotorType.I, RotorType.II, RotorType.III }, "AAA", "ZZZ");
            var result = TestHelpers.Process(machine, "XNNJSANZVAZHBV");
            Assert.Equal("HELLOHOWAREYOU", result);
        }

        [Fact]
        public void Rotors_I_II_III_AAA_ZZZ_Decrypts_Qwerty_Correctly()
        {
            var machine = TestHelpers.BuildMachine(new[] { RotorType.I, RotorType.II, RotorType.III }, "AAA", "ZZZ");
            var result = TestHelpers.Process(machine, "CIJWHI");
            Assert.Equal("QWERTY", result);
        }
    }
}
