using System;
using System.Runtime.Serialization;

namespace PinetreeShop.CQRS.Infrastructure.Exceptions
{
    [Serializable]
    public class AggregateNotFoundException : Exception
    {
        public AggregateNotFoundException(string message) : base(message)
        {
        }
    }
}