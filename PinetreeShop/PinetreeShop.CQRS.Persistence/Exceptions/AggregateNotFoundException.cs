using System;
using System.Runtime.Serialization;

namespace PinetreeShop.CQRS.Persistence.Exceptions
{
    [Serializable]
    public class AggregateNotFoundException : Exception
    {
        public AggregateNotFoundException(string message) : base(message)
        {
        }
    }
}