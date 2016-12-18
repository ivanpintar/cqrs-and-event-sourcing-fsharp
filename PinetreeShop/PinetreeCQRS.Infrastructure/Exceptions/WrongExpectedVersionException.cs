using System;

namespace PinetreeCQRS.Infrastructure.Exceptions
{
    [Serializable]
    public class WrongExpectedVersionException : Exception
    {
        public WrongExpectedVersionException(string message) : base(message)
        {
        }
    }
}