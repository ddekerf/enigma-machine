using System.Linq;
using System.Text;
using EnigmaMachine.Domain.Entities;
using EnigmaMachine.Domain.Factories;
using EnigmaMachine.Domain.Interfaces;
using EnigmaMachine.Domain.ValueObjects;
using Xunit;

namespace EnigmaMachine.Domain.Tests;

public class EnigmaEncodingTests
{
    private static EnigmaMachine BuildMachine(RotorType[] rotorsLeftToRight, string rotorStart, string ringSettings)
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

    private static string Process(EnigmaMachine machine, string data)
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
    public void Rotors_I_III_V_WWW_BBB_Encrypts_Correctly()
    {
        var machine = BuildMachine(new[] { RotorType.I, RotorType.III, RotorType.V }, "WWW", "BBB");
        var result = Process(machine, "HelloHowAreYou");
        Assert.Equal("DXXIQUIJZMNBEH", result);
    }

    [Fact]
    public void Rotors_I_III_V_ABC_XYZ_Encrypts_Correctly()
    {
        var machine = BuildMachine(new[] { RotorType.I, RotorType.III, RotorType.V }, "ABC", "XYZ");
        var result = Process(machine, "HelloHowAreYou");
        Assert.Equal("YNHWFVJEMWGLVY", result);
    }

    [Fact]
    public void Rotors_I_III_IV_ABC_XYZ_Encrypts_Correctly()
    {
        var machine = BuildMachine(new[] { RotorType.I, RotorType.III, RotorType.IV }, "ABC", "XYZ");
        var result = Process(machine, "HelloHowAreYou");
        Assert.Equal("PKWSGCUAYAMUDG", result);
    }

    [Fact]
    public void Rotors_IV_III_V_ABC_XYZ_Encrypts_Correctly()
    {
        var machine = BuildMachine(new[] { RotorType.IV, RotorType.III, RotorType.V }, "ABC", "XYZ");
        var result = Process(machine, "HelloHowAreYou");
        Assert.Equal("ZMDVVLHGBMXMTN", result);
    }

    [Fact]
    public void Rotors_I_II_III_AAA_ZZZ_Encrypts_Correctly()
    {
        var machine = BuildMachine(new[] { RotorType.I, RotorType.II, RotorType.III }, "AAA", "ZZZ");
        var result = Process(machine, "HelloHowAreYou");
        Assert.Equal("XNNJSANZVAZHBV", result);
    }

    [Fact]
    public void Rotors_I_II_III_AAA_ZZZ_Encrypts_Qwerty_Correctly()
    {
        var machine = BuildMachine(new[] { RotorType.I, RotorType.II, RotorType.III }, "AAA", "ZZZ");
        var result = Process(machine, "QWERTY");
        Assert.Equal("CIJWHI", result);
    }

    private class ReflectorB : IReflector
    {
        private const string Wiring = "YRUHQSLDPXNGOKMIEBFZCWVJAT";
        public char Reflect(char input) => Wiring[input - 'A'];
    }
}
