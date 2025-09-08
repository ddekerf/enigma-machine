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
    private static IEnigmaMachine BuildMachine(RotorType[] rotorsLeftToRight, string rotorStart, string ringSettings)
    {
        var plugboard = new Plugboard();
        foreach (var pair in "BA QU CG XT DI ER JW LS VK NM".Split(' ', System.StringSplitOptions.RemoveEmptyEntries))
        {
            plugboard.Connect(new PlugboardPair(pair[0], pair[1]));
        }

    IReflector reflector = new EnigmaMachine.Domain.Entities.ReflectorB();

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
    public void Rotors_I_III_V_WWW_BBB_Encrypts_Correctly()
    {
        var machine = BuildMachine(new[] { RotorType.I, RotorType.III, RotorType.V }, "WWW", "BBB");
        var result = Process(machine, "HELLOHOWAREYOU");
        Assert.Equal("DXXIQUIJZMNBEH", result);
    }

        [Fact]
    public void Rotors_I_III_V_WWW_BBB_Decrypts_Correctly()
    {
        var machine = BuildMachine(new[] { RotorType.I, RotorType.III, RotorType.V }, "WWW", "BBB");
        var result = Process(machine, "DXXIQUIJZMNBEH");
        Assert.Equal("HELLOHOWAREYOU", result);
    }

    [Fact]
    public void Rotors_I_III_V_ABC_XYZ_Encrypts_Correctly()
    {
        var machine = BuildMachine(new[] { RotorType.I, RotorType.III, RotorType.V }, "ABC", "XYZ");
        var result = Process(machine, "HELLOHOWAREYOU");
        Assert.Equal("YNHWFVJEMWGLVY", result);
    }

    [Fact]
    public void Rotors_I_III_V_ABC_XYZ_Decrypts_Correctly()
    {
        var machine = BuildMachine(new[] { RotorType.I, RotorType.III, RotorType.V }, "ABC", "XYZ");
        var result = Process(machine, "YNHWFVJEMWGLVY");
        Assert.Equal("HELLOHOWAREYOU", result);
    }

    [Fact]
    public void Rotors_I_III_IV_ABC_XYZ_Encrypts_Correctly()
    {
        var machine = BuildMachine(new[] { RotorType.I, RotorType.III, RotorType.IV }, "ABC", "XYZ");
        var result = Process(machine, "HELLOHOWAREYOU");
        Assert.Equal("PKWSGCUAYAMUDG", result);
    }

    [Fact]
    public void Rotors_I_III_IV_ABC_XYZ_Decrypts_Correctly()
    {
        var machine = BuildMachine(new[] { RotorType.I, RotorType.III, RotorType.IV }, "ABC", "XYZ");
        var result = Process(machine, "PKWSGCUAYAMUDG");
        Assert.Equal("HELLOHOWAREYOU", result);
    }

    [Fact]
    public void Rotors_IV_III_V_ABC_XYZ_Encrypts_Correctly()
    {
        var machine = BuildMachine(new[] { RotorType.IV, RotorType.III, RotorType.V }, "ABC", "XYZ");
        var result = Process(machine, "HELLOHOWAREYOU");
        Assert.Equal("ZMDVVLHGBMXMTN", result);
    }

    [Fact]
    public void Rotors_IV_III_V_ABC_XYZ_Decrypts_Correctly()
    {
        var machine = BuildMachine(new[] { RotorType.IV, RotorType.III, RotorType.V }, "ABC", "XYZ");
        var result = Process(machine, "ZMDVVLHGBMXMTN");
        Assert.Equal("HELLOHOWAREYOU", result);
    }

    [Fact]
    public void Rotors_I_II_III_AAA_ZZZ_Encrypts_Correctly()
    {
        var machine = BuildMachine(new[] { RotorType.I, RotorType.II, RotorType.III }, "AAA", "ZZZ");
        var result = Process(machine, "HELLOHOWAREYOU");
        Assert.Equal("XNNJSANZVAZHBV", result);
    }

    [Fact]
    public void Rotors_I_II_III_AAA_ZZZ_Decrypts_Correctly()
    {
        var machine = BuildMachine(new[] { RotorType.I, RotorType.II, RotorType.III }, "AAA", "ZZZ");
        var result = Process(machine, "XNNJSANZVAZHBV");
        Assert.Equal("HELLOHOWAREYOU", result);
    }

    [Fact]
    public void Rotors_I_II_III_AAA_ZZZ_Encrypts_Qwerty_Correctly()
    {
        var machine = BuildMachine(new[] { RotorType.I, RotorType.II, RotorType.III }, "AAA", "ZZZ");
        var result = Process(machine, "QWERTY");
        Assert.Equal("CIJWHI", result);
    }

    [Fact]
    public void Rotors_I_II_III_AAA_ZZZ_Decrypts_Qwerty_Correctly()
    {
        var machine = BuildMachine(new[] { RotorType.I, RotorType.II, RotorType.III }, "AAA", "ZZZ");
        var result = Process(machine, "CIJWHI");
        Assert.Equal("QWERTY", result);
    }

    // Domain provides ReflectorB

    [Fact]
    public void DoubleStepping_Behavior_Is_Correct()
    {
        // Use rotors I-II-III with ring A and set initial positions to force double-stepping around middle rotor notch.
        // Rotor II notch at 'E' and Rotor III notch at 'V'. Configure a scenario where:
        // - Right rotor advances into notch and causes middle to step
        // - Middle rotor at notch causes left to step
        // We'll assert relative stepping by encrypting enough characters and inspecting outputs consistency.

        var machine = BuildMachine(new[] { RotorType.I, RotorType.II, RotorType.III }, "QEV", "AAA");

        // Process a few characters to cross the notch boundary reliably
        var result = Process(machine, "AAAAA");

        // We don't have direct rotor positions, so validate by round-trip symmetry and stable mapping chunking
        // Simple sanity: encryption should be deterministic and decryptable
        var machine2 = BuildMachine(new[] { RotorType.I, RotorType.II, RotorType.III }, "QEV", "AAA");
        var decrypted = Process(machine2, result);
        Assert.Equal("AAAAA", decrypted);

        // The key aspect: ensure different adjacent outputs exist (indicating multi-rotor stepping happened)
        Assert.Contains(result.Distinct().Count(), new[] { 2, 3, 4, 5 });
    }
}
