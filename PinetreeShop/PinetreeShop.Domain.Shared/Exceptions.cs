using System;

namespace PinetreeShop.Domain.Shared.Exceptions
{
    [Serializable]
    public class DomainException : Exception
    {
        public DomainException(Guid id, string message) : base($"Product {id}: {message}")
        {
        }
    }

    [Serializable]
    public class AggregateExistsException : DomainException
    {
        public AggregateExistsException(Guid id, string message) : base(id, message)
        {
        }
    }

    [Serializable]
    public class ParameterNullException : DomainException
    {
        public ParameterNullException(Guid id, string message) : base(id, message)
        {
        }
    }
}
