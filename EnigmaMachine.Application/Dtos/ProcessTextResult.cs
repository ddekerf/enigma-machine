using System.Collections.Generic;

namespace EnigmaMachine.Application.Dtos;

public sealed record ProcessTextResult(
    string CipherText,
    IReadOnlyList<MachineStateDto> Steps);
