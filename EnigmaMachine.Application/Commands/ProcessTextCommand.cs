using MediatR;
using EnigmaMachine.Application.Dtos;
using EnigmaMachine.Domain.ValueObjects;

namespace EnigmaMachine.Application.Commands;

public sealed record MachineConfiguration(
    RotorType[] Rotors,
    string RingSettings,
    string InitialPositions,
    string[]? PlugboardPairs);

public sealed record ProcessTextCommand(
    string InputText,
    MachineConfiguration Configuration) : IRequest<ProcessTextResult>;
