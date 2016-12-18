using PinetreeCQRS.Infrastructure.Commands;
using PinetreeCQRS.Infrastructure.Events;
using System;
using System.Collections.Generic;

namespace PinetreeCQRS.Infrastructure
{
    public class ProcessManagerBase : IProcessManager
    {
        private Dictionary<Type, Action<IEvent>> _eventHandlers = new Dictionary<Type, Action<IEvent>>();

        public Guid ProcessId { get; protected set; }

        private int _version = 0;
        public int Version
        {
            get { return _version; }
        }

        private List<IEvent> _uncommittedEvents = new List<IEvent>();
        public IEnumerable<IEvent> UncommittedEvents { get { return _uncommittedEvents; } }

        public void ClearUncommittedEvents()
        {
            _uncommittedEvents.Clear();
        }

        private Dictionary<Type, List<ICommand>> _undispatchedCommands = new Dictionary<Type, List<ICommand>>();
        public Dictionary<Type, List<ICommand>> UndispatchedCommands { get { return _undispatchedCommands; } }

        public void ClearUndispatchedCommands()
        {
            _undispatchedCommands.Clear();
        }

        public void Transition(IEvent evt)
        {
            var eventType = evt.GetType();
            if (_eventHandlers.ContainsKey(eventType))
            {
                _eventHandlers[eventType](evt);
            }
            _version++;
        }

        public void HandleEvent(IEvent evt)
        {
            Transition(evt);
            _uncommittedEvents.Add(new EventProcessed(ProcessId, evt));
        }

        protected void DispatchCommand<TAggregate>(ICommand command)
        {
            var type = typeof(TAggregate);
            if (!_undispatchedCommands.ContainsKey(type))
            {
                _undispatchedCommands[type] = new List<ICommand>();
            }
            _undispatchedCommands[type].Add(command);
        }

        protected void RegisterEventHandler<T>(Action<T> handler) where T : class
        {
            _eventHandlers.Add(typeof(T), o => handler(o as T));
        }
    }
}
