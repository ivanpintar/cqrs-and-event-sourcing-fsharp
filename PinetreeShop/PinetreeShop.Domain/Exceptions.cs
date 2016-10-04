using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinetreeShop.Domain.Exceptions
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
