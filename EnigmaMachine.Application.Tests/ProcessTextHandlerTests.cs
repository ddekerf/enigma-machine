using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EnigmaMachine.Application.Commands;
using EnigmaMachine.Application.Handlers;
using EnigmaMachine.Domain.Entities;
using EnigmaMachine.Domain.Factories;
using EnigmaMachine.Domain.Interfaces;
using EnigmaMachine.Domain.ValueObjects;
using Xunit;

namespace EnigmaMachine.Application.Tests;

public class ProcessTextHandlerTests
{
    private static ProcessTextHandler CreateHandler()
        => new ProcessTextHandler(
            () => new Plugboard(),
            () => new ReflectorB(),
            (rotors, rings, positions, plugboard, reflector) =>
                EnigmaMachineFactory.CreateEnigmaILeftToRight(rotors, rings, positions, plugboard, reflector),
            new DefaultTextTransformer());

    [Fact]
    public async Task ProcessText_ReturnsCipherAndStateSnapshots()
    {
        var handler = CreateHandler();
        var pairs = "BA QU CG XT DI ER JW LS VK NM".Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var config = new MachineConfiguration(
            new[] { RotorType.I, RotorType.III, RotorType.V },
            "BBB",
            "WWW",
            pairs);
        var command = new ProcessTextCommand("HELLOHOWAREYOU", config);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal("DXXIQUIJZMNBEH", result.CipherText);
        Assert.Equal(14, result.Steps.Count);
        Assert.All(result.Steps, s => Assert.Equal(3, s.RotorPositions.Count));
        Assert.All(result.Steps, s => Assert.Equal(10, s.Plugboard.Count));
    }

    [Fact]
    public async Task ProcessText_NonLetterCharactersArePreservedWithoutStates()
    {
        var handler = CreateHandler();
        var config = new MachineConfiguration(
            new[] { RotorType.I, RotorType.II, RotorType.III },
            "AAA",
            "AAA",
            null);
        var command = new ProcessTextCommand("HI THERE!", config);

        var result = await handler.Handle(command, CancellationToken.None);

        // Build expected output using domain components
        var plugboard = new Plugboard();
        var reflector = new ReflectorB();
        var machine = EnigmaMachineFactory.CreateEnigmaILeftToRight(
            new[] { RotorType.I, RotorType.II, RotorType.III },
            "AAA".ToCharArray(),
            "AAA".ToCharArray(),
            plugboard,
            reflector);
        var transformer = new DefaultTextTransformer();
        var encoded = transformer.Encode("HI THERE!");
        var sb = new StringBuilder();
        foreach (var ch in encoded)
        {
            if (!char.IsLetter(ch))
            {
                sb.Append(ch);
                continue;
            }
            sb.Append(machine.ProcessLetter(new Letter(ch)).Character);
        }
        var expected = sb.ToString();

        Assert.Equal(expected, result.CipherText);
        Assert.Equal(7, result.Steps.Count); // only letters processed as steps
    }
}
