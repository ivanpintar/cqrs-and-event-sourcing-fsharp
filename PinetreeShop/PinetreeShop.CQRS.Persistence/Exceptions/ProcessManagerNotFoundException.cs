using System;
using System.Runtime.Serialization;

namespace PinetreeShop.CQRS.Persistence.Exceptions
{
    [Serializable]
    public class ProcessManagerNotFoundException : Exception
    {
        public ProcessManagerNotFoundException(string message) : base(message)
        {
        }
    }
}