using System;
using System.Runtime.Serialization;

namespace PinetreeCQRS.Infrastructure.Exceptions
{
    [Serializable]
    public class AggregateNotFoundException : Exception
    {
        public AggregateNotFoundException(string message) : base(message)
        {
        }
    }
}