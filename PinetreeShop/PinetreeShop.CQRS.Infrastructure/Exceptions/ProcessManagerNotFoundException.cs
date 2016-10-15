using System;
using System.Runtime.Serialization;

namespace PinetreeShop.CQRS.Infrastructure.Exceptions
{
    [Serializable]
    public class ProcessManagerNotFoundException : Exception
    {
        public ProcessManagerNotFoundException(string message) : base(message)
        {
        }
    }
}