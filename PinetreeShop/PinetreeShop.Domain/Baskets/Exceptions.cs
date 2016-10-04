using PinetreeShop.Domain.Exceptions;
using System;

namespace PinetreeShop.Domain.Baskets.Exceptions
{
    public class CheckoutException : DomainException
    {
        public CheckoutException(Guid id, string message) : base(id, message)
        {
        }
    }

    public class CancellationException : DomainException
    {
        public CancellationException(Guid id, string message) : base(id, message)
        {
        }
    }
}
