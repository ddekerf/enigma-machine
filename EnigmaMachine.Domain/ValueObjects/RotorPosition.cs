// RotorPosition.cs (rich value object for UI/diagnostics)
using System;

namespace EnigmaMachine.Domain.ValueObjects
{
    /// <summary>
    /// Immutable rotor position in range 0–25 with wrap-around semantics.
    /// Provides conversions to and from A–Z letters.
    /// </summary>
    public readonly struct RotorPosition : IEquatable<RotorPosition>
    {
        /// <summary>
        /// 0-based position (0..25). 0 corresponds to 'A', 25 corresponds to 'Z'.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Uppercase letter view of the position ('A'..'Z').
        /// </summary>
        public char Letter => (char)('A' + Index);

        public RotorPosition(int index)
        {
            // Normalize to [0,25] using Euclidean modulo
            Index = Normalize(index);
        }

        public static RotorPosition FromChar(char c)
        {
            if (!char.IsLetter(c)) throw new ArgumentOutOfRangeException(nameof(c), "Letter A-Z expected.");
            c = char.ToUpperInvariant(c);
            if (c < 'A' || c > 'Z') throw new ArgumentOutOfRangeException(nameof(c), "Letter A-Z expected.");
            return new RotorPosition(c - 'A');
        }

        public RotorPosition Advance(int steps = 1) => new RotorPosition(Index + steps);
        public RotorPosition Retreat(int steps = 1) => new RotorPosition(Index - steps);

        public override string ToString() => Letter.ToString();

        public bool Equals(RotorPosition other) => Index == other.Index;
        public override bool Equals(object? obj) => obj is RotorPosition rp && Equals(rp);
        public override int GetHashCode() => Index;
        public static bool operator ==(RotorPosition left, RotorPosition right) => left.Equals(right);
        public static bool operator !=(RotorPosition left, RotorPosition right) => !left.Equals(right);

        private static int Normalize(int value)
        {
            int m = value % 26;
            if (m < 0) m += 26;
            return m;
        }
    }
}