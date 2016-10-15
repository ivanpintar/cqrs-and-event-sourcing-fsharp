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

        protected TProcessManager BuildProcessManager<TProcessManager>(IEnumerable<IEvent> events) where TProcessManager : IProcessManager, new()
        {
            var result = new TProcessManager();
            foreach(var evt in events)
            {
                result.Transition(evt);
            }
            
            result.ClearUncommittedEvents();
            result.ClearUndispatchedCommands();
            return result;
        }

        public abstract void SaveProcessManager<TProcessManager>(TProcessManager aggregate) where TProcessManager : IProcessManager;
        public abstract TProcessManager GetProcessManagerById<TProcessManager>(Guid id) where TProcessManager : IProcessManager, new();
    }
}
