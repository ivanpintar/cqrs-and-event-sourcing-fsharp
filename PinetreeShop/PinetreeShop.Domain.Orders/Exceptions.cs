using PinetreeShop.Domain.Shared.Exceptions;
using System;

namespace PinetreeShop.Domain.Tests.Order.Exceptions
{
    [Serializable]
    public class EmptyOrderLinesException : DomainException
    {
        public EmptyOrderLinesException(Guid id, string message) : base(id, message)
        {
        }
    }

    [Serializable]
    public class InvalidOrderStateException : DomainException
    {
        public InvalidOrderStateException(Guid id, string message) : base(id, message)
        {
        }
    }
}
