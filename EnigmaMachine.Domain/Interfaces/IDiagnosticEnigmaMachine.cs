using System.Collections.Generic;

namespace EnigmaMachine.Domain.Interfaces
{
    /// <summary>
    /// Diagnostic/test-only interface to inspect internal rotor positions.
    /// Not intended for application logic.
    /// </summary>
    public interface IDiagnosticEnigmaMachine
    {
        /// <summary>
        /// Snapshot of rotor positions as 0-25 values.
        /// Order: index 0 is the rightmost (fast) rotor, last is the leftmost (slow) rotor.
        /// </summary>
        IReadOnlyList<int> RotorPositions { get; }
    }
}
