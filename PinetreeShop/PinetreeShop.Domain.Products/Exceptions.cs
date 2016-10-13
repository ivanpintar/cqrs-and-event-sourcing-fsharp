using PinetreeShop.Domain.Shared.Exceptions;
using System;

namespace PinetreeShop.Domain.Products.Exceptions
{
    public class ProductCreationException : DomainException
    {
        public ProductCreationException(Guid id, string message) : base(id, message)
        {
        }
    }

    public class QuantityChangeException : DomainException
    {
        public QuantityChangeException(Guid id, string message) : base(id, message)
        {
        }
    }

    public class ProductReservationException : DomainException
    {
        public ProductReservationException(Guid id, string message) : base(id, message)
        {
        }
    }

    public class ProductReservationReleaseException : DomainException
    {
        public ProductReservationReleaseException(Guid id, string message) : base(id, message)
        {
        }
    }
}
