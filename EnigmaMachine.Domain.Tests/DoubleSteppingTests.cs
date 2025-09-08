using System.Linq;
using EnigmaMachine.Domain.Entities;
using EnigmaMachine.Domain.Factories;
using EnigmaMachine.Domain.Interfaces;
using EnigmaMachine.Domain.ValueObjects;
using Xunit;

namespace EnigmaMachine.Domain.Tests;

public class DoubleSteppingTests
{
    private static (IEnigmaMachine m, IDiagnosticEnigmaMachine d) Build(char l, char m, char r, char rl, char rm, char rr)
    {
        var plug = new Plugboard();
        var refl = new ReflectorB();
        // Left-to-right creation for clarity
        var types = new[] { RotorType.I, RotorType.II, RotorType.III };
        var rings = new[] { rl, rm, rr }; // ring settings L-M-R
        var pos = new[] { l, m, r };       // initial positions L-M-R
        var machine = EnigmaMachineFactory.CreateEnigmaILeftToRight(types, rings, pos, plug, refl);
        return (machine, (IDiagnosticEnigmaMachine)machine);
    }

    [Fact]
    public void MiddleRotor_DoubleSteps_When_RightWasAtNotch_And_MiddleAtNotch()
    {
    // Use Enigma I rotors I-II-III; Notches: II at 'E', III at 'V'.
    // Set right rotor at its notch BEFORE the first press to trigger middle stepping on the first keypress.
    // Middle at 'D' so after first press (carry) it becomes 'E' (at its own notch), setting up double-step on the second press.
    var (m1, d1) = Build(l: 'A', m: 'D', r: 'V', rl: 'A', rm: 'A', rr: 'A');

    // Press 1: Right steps V->W, middle steps (carry) because right WAS at notch before the press.
        m1.ProcessLetter(new Letter('A'));
        var p1 = d1.RotorPositions.ToArray(); // order: [R, M, L]
    // Right has stepped from V->W, middle from D->E
    Assert.Equal('W' - 'A', p1[0]);
        Assert.Equal('E' - 'A', p1[1]);

    // Press 2: Double-step: middle is at its notch (E) before the press -> middle steps again AND causes left to step; right steps as always.
        m1.ProcessLetter(new Letter('A'));
        var p2 = d1.RotorPositions.ToArray();
    Assert.Equal('X' - 'A', p2[0]); // right W->X
        Assert.Equal('F' - 'A', p2[1]); // middle E->F (double step)
        Assert.Equal(('A' - 'A' + 1) % 26, p2[2]); // left A->B
    }
}
