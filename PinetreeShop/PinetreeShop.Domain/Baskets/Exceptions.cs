using PinetreeShop.Domain.Exceptions;
using System;

namespace PinetreeShop.Domain.Baskets.Exceptions
{
    public class InvalidStateException : DomainException
    {
        public InvalidStateException(Guid id, string message) : base(id, message)
        {
        }
    }

    public class EmtpyBasketException : DomainException
    {
        public EmtpyBasketException(Guid id, string message) : base(id, message)
        {
        }
    }
}
