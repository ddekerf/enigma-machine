using System;

namespace EnigmaMachine.Domain.Exceptions
{
    /// <summary>
    /// Thrown when a domain operation violates an invariant (e.g., exceeding limits,
    /// attempting conflicting connections, etc.).
    /// </summary>
    public class DomainOperationException : DomainException
    {
        public DomainOperationException() : base("A domain operation rule was violated.") { }
        public DomainOperationException(string message) : base(message) { }
        public DomainOperationException(string message, Exception inner) : base(message, inner) { }
    }
}
