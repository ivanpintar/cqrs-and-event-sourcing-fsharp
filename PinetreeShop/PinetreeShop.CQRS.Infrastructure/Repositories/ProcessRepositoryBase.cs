using PinetreeShop.CQRS.Infrastructure.Events;
using System;
using System.Collections.Generic;

namespace PinetreeShop.CQRS.Infrastructure.Repositories
{
    public abstract class ProcessRepositoryBase : IProcessRepository
    {        
        protected int CalculateExpectedVersion<T>(IProcess process, List<T> events)
        {
            return process.Version - events.Count;
        }

        protected TResult BuildProcess<TResult>(IEnumerable<IEvent> events) where TResult : IProcess, new()
        {
            var result = new TResult();
            foreach (var evt in events)
            {
                result.Transition(evt);
            }

            // clear the events and commands so that they are not re-dispatched
            result.ClearUncommittedEvents();
            result.ClearUndispatchedCommands();

            return result;
        }
        
        public abstract void SaveProcess<TProcess>(TProcess process) where TProcess : IProcess;
        public abstract TResult GetProcessById<TResult>(Guid id) where TResult : IProcess, new();
    }
}
