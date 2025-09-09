# EnigmaMachine.Domain

Core domain logic for simulating the WWII Enigma machine. This library follows Clean Architecture/DDD principles and contains only business logic (no UI/infrastructure).

Target framework: .NET 8.0

## Project structure

- Entities
  - `EnigmaMachine.cs` – Aggregate root that wires plugboard, rotors, and reflector, including double‑stepping rotor logic and processing order.
  - `Plugboard.cs` – Manages letter swaps and exposes connections.
  - `Reflector.cs` – `ReflectorB` implementation using historical wiring (YRUHQSLDPXNGOKMIEBFZCWVJAT).
  - `Rotor.cs` – Rotor implementation with forward and reverse paths, ring settings, positions, and notch.
- Factories
  - `EnigmaMachineFactory.cs` – Builds a configured Enigma I using three rotors and provided plugboard/reflector.
  - `RotorFactory.cs` – Historical rotor presets (I–V) with wiring and notch.
- ValueObjects
  - `Letter.cs`, `PlugboardPair.cs`, `RotorType.cs`, `RotorPosition.cs` (UI-friendly diagnostics).
- Interfaces
  - `IEnigmaMachine.cs`, `IPlugboard.cs`, `IReflector.cs`, `IRotor.cs`, `IDiagnosticEnigmaMachine.cs`.
- Exceptions
  - `DomainException.cs`.

## Features

- Historical rotor presets I–V (wiring + notch).
- Correct Enigma stepping: rotors step before encoding, with middle rotor double‑stepping behavior.
- Forward and reverse rotor traversal around a reflector.
- Plugboard swaps pre‑ and post‑processing.
- Input normalized via `Letter` (letters only) and machine output in uppercase A–Z.
 - Diagnostics interface exposes rotor positions as both integers and `RotorPosition` value objects for UI rendering.

## Quick start

Add a reference to this project from your application or tests, then assemble a machine. Example with `ReflectorB` and a few plugboard pairs:

```csharp
using EnigmaMachine.Domain.Entities;
using EnigmaMachine.Domain.Factories;
using EnigmaMachine.Domain.Interfaces;
using EnigmaMachine.Domain.ValueObjects;

// Simple Reflector B implementation (historical wiring)
var reflector = new ReflectorB();

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
  ringSettings: "XYZ".ToCharArray().Reverse().ToArray(),
  initialPositions: "ABC".ToCharArray().Reverse().ToArray(),
  plugboard,
  reflector);

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
- `EnigmaMachineFactory.CreateEnigmaI` expects parameters for rotors/rings/positions in right–to–left order (fast/rightmost first).
- Prefer `CreateEnigmaILeftToRight` to pass arrays in human/historical order (left/slow to right/fast); it reverses internally.
- All processing uses uppercase internally; decrypted outputs will be uppercase.
- Provide a proper `IReflector` (like Reflector B) for historically accurate behavior.
 - Constructor validation: EnigmaMachine requires at least one rotor and rejects null entries in the rotor list.

## Tests

Unit tests live in `EnigmaMachine.Domain.Tests` and cover multiple rotor/ring/position combinations (including decrypt/round‑trip cases).

Run from the repo root:

```powershell
dotnet test --nologo -v minimal
```

## Implementation details

- Rotor stepping occurs before encoding each letter; middle rotor double‑steps when at notch or when the right rotor was at its notch.
- Rotors implement both `ProcessLetter` (forward) and `ProcessBackward` (reverse) paths.
 - `IDiagnosticEnigmaMachine`:
   - `IReadOnlyList<int> RotorPositions` – 0–25 indices; index 0 is the rightmost (fast) rotor.
   - `IReadOnlyList<RotorPosition> RotorPositionsView` – UI-friendly positions with A–Z letters.
- `RotorFactory` wiring and notch positions:
  - I:  EKMFLGDQVZNTOWYHXUSPAIBRCJ (notch Q)
  - II: AJDKSIRUXBLHWTMCQGZNPYFVOE (notch E)
  - III: BDFHJLCPRTXVZNYEIWGAKMUSQO (notch V)
  - IV: ESOVPZJAYQUIRHXLNFTGKDCMWB (notch J)
  - V:  VZBRGITYUPSDNHLXAWMJQOFECK (notch Z)

## Scope

This project is the domain layer only. Add UI, CLI, or persistence in separate projects to keep concerns separated.