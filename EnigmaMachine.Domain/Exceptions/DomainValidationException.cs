using System;

namespace EnigmaMachine.Domain.Exceptions
{
    /// <summary>
    /// Thrown when invalid arguments, configuration, or value object state are supplied
    /// to domain entities / value objects.
    /// </summary>
    public class DomainValidationException : DomainException
    {
        public DomainValidationException() : base("A domain validation error occurred.") { }
        public DomainValidationException(string message) : base(message) { }
        public DomainValidationException(string message, Exception inner) : base(message, inner) { }
    }
}
