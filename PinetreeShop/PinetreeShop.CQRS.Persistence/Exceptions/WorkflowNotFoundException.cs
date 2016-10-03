using System;
using System.Runtime.Serialization;

namespace PinetreeShop.CQRS.Persistence.Exceptions
{
    [Serializable]
    public class WorkflowNotFoundException : Exception
    {
        public WorkflowNotFoundException(string message) : base(message)
        {
        }

        public WorkflowNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}