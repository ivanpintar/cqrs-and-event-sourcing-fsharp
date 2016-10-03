using PinetreeShop.CQRS.Infrastructure.CommandsAndEvents;
using System;
using System.Collections.Generic;

namespace PinetreeShop.CQRS.Infrastructure
{
    public interface IWorkflow
    {
        Guid Id { get; }
        int Version { get; }
        IEnumerable<IEvent> UncommittedEvents { get; }
        void ClearUncommittedEvents();
        IEnumerable<ICommand> UndispatchedCommands { get; }
        void ClearUndispatchedCommands();
        void Transition(IEvent evt);
    }
}
