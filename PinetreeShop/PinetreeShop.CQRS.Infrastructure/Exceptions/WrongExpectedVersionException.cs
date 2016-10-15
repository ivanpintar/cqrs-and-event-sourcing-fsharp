using System;

namespace PinetreeShop.CQRS.Infrastructure.Exceptions
{
    [Serializable]
    public class WrongExpectedVersionException : Exception
    {
        public WrongExpectedVersionException(string message) : base(message)
        {
        }
    }
}