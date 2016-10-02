using System;
using System.Runtime.Serialization;

namespace PinetreeShop.CQRS.Persistence.Exceptions
{
    [Serializable]
    internal class WrongExpectedVersionException : Exception
    {
        public WrongExpectedVersionException()
        {
        }

        public WrongExpectedVersionException(string message) : base(message)
        {
        }

        public WrongExpectedVersionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WrongExpectedVersionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}