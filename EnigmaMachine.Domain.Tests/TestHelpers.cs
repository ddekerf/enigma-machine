using System.Linq;
using System.Text;
using EnigmaMachine.Domain.Entities;
using EnigmaMachine.Domain.Factories;
using EnigmaMachine.Domain.Interfaces;
using EnigmaMachine.Domain.ValueObjects;

namespace EnigmaMachine.Domain.Tests;

internal static class TestHelpers
{
    public static IEnigmaMachine BuildMachine(RotorType[] rotorsLeftToRight, string rotorStart, string ringSettings)
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

    public static string Process(IEnigmaMachine machine, string data)
    {
        var sb = new StringBuilder();
        foreach (var ch in data)
        {
            var letter = new Letter(ch);
            sb.Append(machine.ProcessLetter(letter).Character);
        }
        return sb.ToString();
    }
}
