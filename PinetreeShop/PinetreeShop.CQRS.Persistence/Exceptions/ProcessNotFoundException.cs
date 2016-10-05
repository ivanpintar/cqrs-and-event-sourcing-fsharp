using System;
using System.Runtime.Serialization;

namespace PinetreeShop.CQRS.Persistence.Exceptions
{
    [Serializable]
    public class ProcessNotFoundException : Exception
    {
        public ProcessNotFoundException(string message) : base(message)
        {
        }

        public ProcessNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}