using PinetreeShop.CQRS.Infrastructure.Events;
using System;
using System.Collections.Generic;

namespace PinetreeShop.CQRS.Infrastructure.Repositories
{
    public abstract class ProcessManagerRepositoryBase : IProcessManagerRepository
    {
        protected int CalculateExpectedVersion<T>(IProcessManager processManager, List<T> events)
        {
            return processManager.Version - events.Count;
        }

        protected TResult BuildProcessManager<TResult>(IEnumerable<Events.IEvent> events) where TResult : IProcessManager, new()
        {
            var result = new TResult();
            foreach(var evt in events)
            {
                result.Transition(evt);
            }
            
            result.ClearUncommittedEvents();
            result.ClearUndispatchedCommands();
            return result;
        }

        public abstract void SaveProcessManager<TProcessManager>(TProcessManager aggregate) where TProcessManager : IProcessManager;
        public abstract TResult GetProcessManagerById<TResult>(Guid id) where TResult : IProcessManager, new();
    }
}
