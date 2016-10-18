using PinetreeShop.Domain.Shared.Exceptions;
using System;

namespace PinetreeShop.Domain.Products.Exceptions
{
    [Serializable]
    public class ProductCreationException : DomainException
    {
        public ProductCreationException(Guid id, string message) : base(id, message)
        {
        }
    }

    [Serializable]
    public class QuantityChangeException : DomainException
    {
        public QuantityChangeException(Guid id, string message) : base(id, message)
        {
        }
    }

    [Serializable]
    public class ProductReservationException : DomainException
    {
        public ProductReservationException(Guid id, string message) : base(id, message)
        {
        }
    }

    [Serializable]
    public class ProductReservationReleaseException : DomainException
    {
        public ProductReservationReleaseException(Guid id, string message) : base(id, message)
        {
        }
    }
}
