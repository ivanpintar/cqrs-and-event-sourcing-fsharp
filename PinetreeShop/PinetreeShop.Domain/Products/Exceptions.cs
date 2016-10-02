using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinetreeShop.Domain.Products.Exceptions
{
    public class ProductException : Exception
    {
        public ProductException(Guid id, string message) : base($"Product {id}: {message}")
        {
        }
    }

    public class ProductExistsException : ProductException
    {
        public ProductExistsException(Guid id, string message) : base(id, message)
        {
        }
    }

    public class QuantityChangeException : ProductException
    {
        public QuantityChangeException(Guid id, string message) : base(id, message)
        {
        }
    }

    public class ProductReservationException : ProductException
    {
        public ProductReservationException(Guid id, string message) : base(id, message)
        {
        }
    }

    public class ProductReservationReleaseException : ProductException
    {
        public ProductReservationReleaseException(Guid id, string message) : base(id, message)
        {
        }
    }
}
