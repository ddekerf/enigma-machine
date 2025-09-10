# Enigma Machine (Domain)

<!-- Coverage badge (configure Codecov and update link) -->
![coverage](https://img.shields.io/badge/coverage-pending-lightgrey)

This repository contains a .NET 8 domain library that models the WWII Enigma machine. It follows DDD/Clean Architecture principles: the domain is infrastructure-free and test-driven.

## Projects
- EnigmaMachine.Domain: core business logic (rotors, plugboard, reflector, machine, factories, value objects)
- EnigmaMachine.Domain.Tests: xUnit tests covering encoding scenarios and stepping

## Build and test

```powershell
# from repo root
 dotnet restore
 dotnet build -c Release
 dotnet test -c Release --nologo -v minimal
```

## Scope
- Enigma I (3 rotors) with Reflector B
- Historical rotor wirings I–V and correct double-stepping behavior
- UI-friendly diagnostics via `IDiagnosticEnigmaMachine` (see Domain README)

## Notes
- Non-thread-safe: create separate instances per concurrent operation
- Inputs are normalized to uppercase A–Z

See `EnigmaMachine.Domain/README.md` for domain details.

## Example (left-to-right overload)

```csharp
using EnigmaMachine.Domain.Entities;
using EnigmaMachine.Domain.Factories;
using EnigmaMachine.Domain.Interfaces;
using EnigmaMachine.Domain.ValueObjects;

var plugboard = new Plugboard();
foreach (var pair in new[] { "BA", "QU", "CG" })
	plugboard.Connect(new PlugboardPair(pair[0], pair[1]));

var reflector = new ReflectorB();

// Supply left-to-right (slow to fast) order; API reverses to machine order internally
var machine = EnigmaMachineFactory.CreateEnigmaILeftToRight(
	new[] { RotorType.I, RotorType.III, RotorType.V },
	new[] { 'A', 'B', 'C' },          // ring settings L→R
	new[] { 'X', 'Y', 'Z' },          // initial positions L→R
	plugboard,
	reflector);

// Process text
var input = "HELLO WORLD";
var sb = new System.Text.StringBuilder();
foreach (var ch in input)
{
	if (!char.IsLetter(ch)) { sb.Append(ch); continue; }
	var letter = new Letter(ch);
	sb.Append(machine.ProcessLetter(letter).Character);
}
var cipher = sb.ToString();

// Diagnostics (optional): cast to IDiagnosticEnigmaMachine to inspect rotor positions
var diag = (IDiagnosticEnigmaMachine)machine;
// 0–25 indices, rightmost (fast) at index 0
var numeric = diag.RotorPositions;
// Rich view for UI: A–Z letters and normalized indices
var view = diag.RotorPositionsView; // RotorPosition value objects
```

Notes:
- The `EnigmaMachine` constructor validates the rotor list: it must be non-null, non-empty, and contain no null entries.
