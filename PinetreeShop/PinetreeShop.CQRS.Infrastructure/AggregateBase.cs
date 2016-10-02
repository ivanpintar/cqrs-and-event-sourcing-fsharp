using System;
using System.Collections.Generic;

namespace PinetreeShop.CQRS.Infrastructure
{
    public class AggregateBase : IAggregate
    {
        private Dictionary<Type, Action<IEvent>> _routes = new Dictionary<Type, Action<IEvent>>();

        public Guid Id { get; protected set; }

        private int _version = -1;
        public int Version
        {
            get { return _version; }
        }

        private List<IEvent> _uncommittedEvents = new List<IEvent>();
        public IEnumerable<IEvent> UncommittedEvents { get { return _uncommittedEvents; } }

        public void RaiseEvent(IEvent evt)
        {
            ApplyEvent(evt);
            _uncommittedEvents.Add(evt);
        }

        public void ClearUncommittedEvents()
        {
            _uncommittedEvents.Clear();
        }

        public void ApplyEvent(IEvent evt)
        {
            var eventType = evt.GetType();
            if (_routes.ContainsKey(eventType))
            {
                _routes[eventType](evt);
            }
            _version++;
        }

        protected void RegisterTransition<T>(Action<T> transition) where T : class
        {
            _routes.Add(typeof(T), o => transition(o as T));
        }
    }
}
