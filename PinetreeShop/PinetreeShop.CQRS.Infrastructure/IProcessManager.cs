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

        IEnumerable<Events.IEvent> UncommittedEvents { get; }
        void ClearUncommittedEvents();

        IEnumerable<ICommand> UndispatchedCommands { get; }
        void ClearUndispatchedCommands();

        void Transition(Events.IEvent evt);    
    }
}
