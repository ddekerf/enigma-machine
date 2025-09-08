# EnigmaMachine.Domain

Core domain logic for simulating the WWII Enigma machine. This library follows Clean Architecture/DDD principles and contains only business logic (no UI/infrastructure).

Target framework: .NET 6.0

## Project structure

- Entities
  - `EnigmaMachine.cs` – Aggregate root that wires plugboard, rotors, and reflector, including double‑stepping rotor logic and processing order.
  - `Plugboard.cs` – Manages letter swaps and exposes connections.
  - `Reflector.cs` – Minimal placeholder reflector; provide a concrete wiring (e.g., Reflector B) via `IReflector` for real use.
  - `Rotor.cs` – Rotor implementation with forward and reverse paths, ring settings, positions, and notch.
- Factories
  - `EnigmaMachineFactory.cs` – Builds a configured Enigma I using three rotors and provided plugboard/reflector.
  - `RotorFactory.cs` – Historical rotor presets (I–V) with wiring and notch.
- ValueObjects
  - `Letter.cs`, `PlugboardPair.cs`, `RotorPosition.cs`, `RotorType.cs`.
- Interfaces
  - `IEnigmaMachine.cs`, `IPlugboard.cs`, `IReflector.cs`, `IRotor.cs`.
- Exceptions
  - `DomainException.cs`.

## Features

- Historical rotor presets I–V (wiring + notch).
- Correct Enigma stepping: rotors step before encoding, with middle rotor double‑stepping behavior.
- Forward and reverse rotor traversal around a reflector.
- Plugboard swaps pre‑ and post‑processing.
- Input normalized via `Letter` (letters only) and machine output in uppercase A–Z.

## Quick start

Add a reference to this project from your application or tests, then assemble a machine. Example with Reflector B wiring and a few plugboard pairs:

```csharp
using EnigmaMachine.Domain.Entities;
using EnigmaMachine.Domain.Factories;
using EnigmaMachine.Domain.Interfaces;
using EnigmaMachine.Domain.ValueObjects;

// Simple Reflector B implementation (historical wiring)
class ReflectorB : IReflector
{
    private const string Wiring = "YRUHQSLDPXNGOKMIEBFZCWVJAT";
    public char Reflect(char input) => Wiring[input - 'A'];
}

// Build a plugboard from pairs
var plugboard = new Plugboard();
foreach (var pair in new[] { "BA", "QU", "CG" })
{
    plugboard.Connect(new PlugboardPair(pair[0], pair[1]));
}

// Create Enigma I with three rotors (left→right in call),
// ring settings and start positions as strings (A–Z)
var machine = EnigmaMachineFactory.CreateEnigmaI(
    new[] { RotorType.I, RotorType.III, RotorType.V },
    rings: "XYZ".ToCharArray().Reverse().ToArray(),
    initialPositions: "ABC".ToCharArray().Reverse().ToArray(),
    plugboard,
    reflector: new ReflectorB());

// Process data (output is uppercase)
string input = "HelloHowAreYou";
var sb = new StringBuilder();
foreach (var ch in input)
{
    var letter = new Letter(ch);
    sb.Append(machine.ProcessLetter(letter).Character);
}
var cipher = sb.ToString();
```

Notes:
- `EnigmaMachineFactory.CreateEnigmaI` expects parameters for rotors/rings/positions in right‑to‑left order. The example reverses left‑to‑right strings to match this.
- All processing uses uppercase internally; decrypted outputs will be uppercase.
- Provide a proper `IReflector` (like Reflector B) for historically accurate behavior.

## Tests

Unit tests live in `EnigmaMachine.Domain.Tests` and cover multiple rotor/ring/position combinations (including decrypt/round‑trip cases).

Run from the repo root:

```powershell
dotnet test --nologo -v minimal
```

## Implementation details

- Rotor stepping occurs before encoding each letter; middle rotor double‑steps when at notch or when the right rotor was at its notch.
- Rotors implement both `ProcessLetter` (forward) and `ProcessBackward` (reverse) paths.
- `RotorFactory` wiring and notch positions:
  - I:  EKMFLGDQVZNTOWYHXUSPAIBRCJ (notch Q)
  - II: AJDKSIRUXBLHWTMCQGZNPYFVOE (notch E)
  - III: BDFHJLCPRTXVZNYEIWGAKMUSQO (notch V)
  - IV: ESOVPZJAYQUIRHXLNFTGKDCMWB (notch J)
  - V:  VZBRGITYUPSDNHLXAWMJQOFECK (notch Z)

## Scope

This project is the domain layer only. Add UI, CLI, or persistence in separate projects to keep concerns separated.