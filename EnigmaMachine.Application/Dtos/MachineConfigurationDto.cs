using System.Collections.Generic;
using EnigmaMachine.Domain.ValueObjects;

namespace EnigmaMachine.Application.Dtos;

/// <summary>
/// Echo of the machine configuration a message was processed with,
/// so a response is self-describing without the original request.
/// </summary>
public sealed record MachineConfigurationDto(
    IReadOnlyList<RotorType> Rotors,
    string RingSettings,
    string InitialPositions,
    string Reflector,
    IReadOnlyList<PlugboardPairDto> Plugboard);
