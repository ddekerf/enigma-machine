namespace EnigmaMachine.Application.Dtos;

/// <summary>
/// Data transfer object representing a pair of letters connected on the plugboard.
/// </summary>
public sealed record PlugboardPairDto(char First, char Second);
