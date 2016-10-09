using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;
using System;
using System.Collections.Generic;

namespace PinetreeShop.CQRS.Infrastructure
{
    public class ProcessManagerBase : IProcessManager
    {
        private Dictionary<Type, Action<Events.IEvent>> _eventHandlers = new Dictionary<Type, Action<Events.IEvent>>();

        public Guid ProcessId { get; protected set; }

        private int _version = -1;
        public int Version
        {
            get { return _version; }
        }

        private List<Events.IEvent> _uncommittedEvents = new List<Events.IEvent>();
        public IEnumerable<Events.IEvent> UncommittedEvents { get { return _uncommittedEvents; } }

        public void ClearUncommittedEvents()
        {
            _uncommittedEvents.Clear();
        }

        private List<ICommand> _undispatchedCommands = new List<ICommand>();
        public IEnumerable<ICommand> UndispatchedCommands { get { return _undispatchedCommands; } }

        public void ClearUndispatchedCommands()
        {
            _undispatchedCommands.Clear();
        }

        public void Transition(Events.IEvent evt)
        {
            var eventType = evt.GetType();
            if (_eventHandlers.ContainsKey(eventType))
            {
                _eventHandlers[eventType](evt);
            }
            _version++;
        }

        protected void HandleEvent(Events.IEvent evt)
        {
            Transition(evt);
            _uncommittedEvents.Add(evt);
        }

        protected void DispatchCommand(ICommand command)
        {
            _undispatchedCommands.Add(command);
        }

        protected void RegisterEventHandler<T>(Action<T> handler) where T : class
        {
            _eventHandlers.Add(typeof(T), o => handler(o as T));
        }
    }
}
