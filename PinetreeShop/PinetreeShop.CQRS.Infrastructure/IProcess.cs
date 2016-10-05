using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.CQRS.Infrastructure.Commands;
using System;
using System.Collections.Generic;

namespace PinetreeShop.CQRS.Infrastructure
{
    public interface IProcess
    {
        Guid ProcessId { get; }
        int Version { get; }
        IEnumerable<IEvent> UncommittedEvents { get; }
        void ClearUncommittedEvents();
        IEnumerable<ICommand> UndispatchedCommands { get; }
        void ClearUndispatchedCommands();
        void Transition(IEvent evt);
    }
}
