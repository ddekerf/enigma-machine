using System.Collections.Generic;

namespace EnigmaMachine.Application.Dtos;

public sealed record MachineStateDto(
    char Input,
    char Output,
    IReadOnlyList<char> RotorPositions);
