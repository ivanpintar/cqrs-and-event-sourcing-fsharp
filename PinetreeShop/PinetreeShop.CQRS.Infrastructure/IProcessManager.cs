using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;
using System;
using System.Collections.Generic;

namespace PinetreeShop.CQRS.Infrastructure
{
    public interface IProcessManager
    {
        Guid ProcessId { get; }
        int Version { get; }

        IEnumerable<IEvent> UncommittedEvents { get; }
        void ClearUncommittedEvents();

        Dictionary<Type, List<ICommand>> UndispatchedCommands { get; }
        void ClearUndispatchedCommands();

        void Transition(IEvent evt);    
    }
}
