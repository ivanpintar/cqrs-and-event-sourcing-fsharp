using PinetreeCQRS.Infrastructure.Commands;
using PinetreeCQRS.Infrastructure.Events;
using System;
using System.Collections.Generic;

namespace PinetreeCQRS.Infrastructure
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
