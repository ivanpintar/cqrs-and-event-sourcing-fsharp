using PinetreeShop.Domain.Shared.Exceptions;
using System;

namespace PinetreeShop.Domain.Baskets.Exceptions
{
    [Serializable]
    public class InvalidStateException : DomainException
    {
        public InvalidStateException(Guid id, string message) : base(id, message)
        {
        }
    }

    [Serializable]
    public class EmtpyBasketException : DomainException
    {
        public EmtpyBasketException(Guid id, string message) : base(id, message)
        {
        }
    }
}
