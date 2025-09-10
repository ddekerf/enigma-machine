using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using EnigmaMachine.Application.Commands;
using EnigmaMachine.Application.Dtos;
using EnigmaMachine.Domain.Entities;
using EnigmaMachine.Domain.Factories;
using EnigmaMachine.Domain.Interfaces;
using EnigmaMachine.Domain.ValueObjects;

namespace EnigmaMachine.Application.Handlers;

public sealed class ProcessTextHandler : IRequestHandler<ProcessTextCommand, ProcessTextResult>
{
    private readonly Func<IPlugboard> _plugboardFactory;
    private readonly Func<IReflector> _reflectorFactory;
    private readonly Func<RotorType[], char[], char[], IPlugboard, IReflector, IEnigmaMachine> _machineFactory;

    public ProcessTextHandler(
        Func<IPlugboard> plugboardFactory,
        Func<IReflector> reflectorFactory,
        Func<RotorType[], char[], char[], IPlugboard, IReflector, IEnigmaMachine> machineFactory)
    {
        _plugboardFactory = plugboardFactory;
        _reflectorFactory = reflectorFactory;
        _machineFactory = machineFactory;
    }

    public Task<ProcessTextResult> Handle(ProcessTextCommand request, CancellationToken cancellationToken)
    {
        var cfg = request.Configuration;

        var plugboard = _plugboardFactory();
        if (cfg.PlugboardPairs != null)
        {
            foreach (var pair in cfg.PlugboardPairs)
            {
                if (pair?.Length >= 2)
                {
                    plugboard.Connect(new PlugboardPair(pair[0], pair[1]));
                }
            }
        }

        var reflector = _reflectorFactory();
        var machine = _machineFactory(
            cfg.Rotors,
            cfg.RingSettings.ToUpperInvariant().ToCharArray(),
            cfg.InitialPositions.ToUpperInvariant().ToCharArray(),
            plugboard,
            reflector);

        var diag = (IDiagnosticEnigmaMachine)machine;
        var transformer = new DefaultTextTransformer();
        var encoded = transformer.Encode(request.InputText ?? string.Empty);

        var sb = new StringBuilder();
        var states = new List<MachineStateDto>();

        foreach (var ch in encoded)
        {
            if (!char.IsLetter(ch))
            {
                sb.Append(ch);
                continue;
            }

            var inputLetter = new Letter(ch);
            var output = machine.ProcessLetter(inputLetter).Character;
            sb.Append(output);

            var positions = diag.RotorPositionsView.Select(rp => rp.Letter).ToArray();
            var plugs = plugboard
                .GetConnections()
                .Select(p => new PlugboardPairDto(p.FirstLetter, p.SecondLetter))
                .ToArray();
            states.Add(new MachineStateDto(ch, output, positions, plugs));
        }

        var result = new ProcessTextResult(sb.ToString(), states);
        return Task.FromResult(result);
    }
}
