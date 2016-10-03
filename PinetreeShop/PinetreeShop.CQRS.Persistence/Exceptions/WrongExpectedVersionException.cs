using System;
using System.Runtime.Serialization;

namespace PinetreeShop.CQRS.Persistence.Exceptions
{
    [Serializable]
    public class WrongExpectedVersionException : Exception
    {
        public WrongExpectedVersionException(string message) : base(message)
        {
        }
    }
}