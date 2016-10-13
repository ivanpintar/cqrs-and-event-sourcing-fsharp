using System;

namespace PinetreeShop.Domain.Shared.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(Guid id, string message) : base($"Product {id}: {message}")
        {
        }
    }

    public class AggregateExistsException : DomainException
    {
        public AggregateExistsException(Guid id, string message) : base(id, message)
        {
        }
    }

    public class ParameterNullException : DomainException
    {
        public ParameterNullException(Guid id, string message) : base(id, message)
        {
        }
    }
}
