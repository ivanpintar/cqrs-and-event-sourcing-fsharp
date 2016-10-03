using PinetreeShop.CQRS.Infrastructure.CommandsAndEvents;
using System;
using System.Collections.Generic;

namespace PinetreeShop.CQRS.Infrastructure
{
    public class WorkflowBase : IWorkflow
    {
        private Dictionary<Type, Action<IEvent>> _eventHandlers = new Dictionary<Type, Action<IEvent>>();

        public Guid Id { get; protected set; }

        private int _version = -1;
        public int Version
        {
            get { return _version; }
        }

        private List<IEvent> _uncommittedEvents = new List<IEvent>();
        public IEnumerable<IEvent> UncommittedEvents { get { return _uncommittedEvents; } }

        private List<ICommand> _undispatchedCommands = new List<ICommand>();
        public IEnumerable<ICommand> UndispatchedCommands { get { return _undispatchedCommands; } }
        
        public void ClearUncommittedEvents()
        {
            _uncommittedEvents.Clear();
        }

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

        protected void RaiseEvent(IEvent evt)
        {
            Transition(evt);
            _uncommittedEvents.Add(evt);
        }
        
        protected void Dispatch(ICommand command)
        {
            _undispatchedCommands.Add(command);
        }        

        protected void RegisterEventHandler<T>(Action<T> handler) where T : class
        {
            _eventHandlers.Add(typeof(T), o => handler(o as T));
        }
    }
}
