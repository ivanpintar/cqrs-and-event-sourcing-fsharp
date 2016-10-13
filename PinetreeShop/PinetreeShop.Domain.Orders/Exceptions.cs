using PinetreeShop.Domain.Shared.Exceptions;
using System;

namespace PinetreeShop.Domain.Tests.Order.Exceptions
{
    public class EmptyOrderLinesException : DomainException
    {
        public EmptyOrderLinesException(Guid id, string message) : base(id, message)
        {
        }
    }

    public class InvalidOrderStateException : DomainException
    {
        public InvalidOrderStateException(Guid id, string message) : base(id, message)
        {
        }
    }
}
