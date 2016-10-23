using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinetreeShop.CQRS.Infrastructure.Events
{
    public class EventStreamListener
    {
        private int _lastEventNumber = 0;

        private Dictionary<string, object> _eventHandlers = new Dictionary<string, object>();
        private IEventStore _eventStore;

        public EventStreamListener(IEventStore eventStore)
        {
            _eventStore = eventStore;
            _lastEventNumber = 0;
        }

        public void ReadAndHandleLatestEvents(int lastEventNumber)
        {
            _lastEventNumber = lastEventNumber;
            var events = _eventStore.GetEvents(_lastEventNumber).ToList();
            ProcessEvents(events);
        }

        public void ReadAndHandleLatestEvents<TAggregate>(int lastEventNumber) where TAggregate : IAggregate
        {
            _lastEventNumber = lastEventNumber;
            var events = _eventStore.GetEvents<TAggregate>(_lastEventNumber).ToList();
            ProcessEvents(events);
        }

        private void ProcessEvents(List<IEvent> events)
        {
            foreach (var evt in events)
            {
                var key = evt.GetType().Name;
                if (_eventHandlers.ContainsKey(key))
                {
                    (_eventHandlers[key] as dynamic)((dynamic)evt);
                }

                _lastEventNumber = evt.Metadata.EventNumber;
            }
        }

        public void RegisterEventHandler<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            var key = typeof(TEvent).Name;
            _eventHandlers.Add(key, handler);
        }
    }
}
