using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinetreeShop.CQRS.Infrastructure.Events
{
    public class EventStreamListener
    {
        private int _lastReadEventId = 0;

        private Dictionary<string, object> _eventHandlers = new Dictionary<string, object>();
        private IEventStore _eventStore;

        public EventStreamListener(IEventStore eventStore, int lastReadEventId = 0)
        {
            _eventStore = eventStore;
            _lastReadEventId = lastReadEventId;
        }

        public void ReadAndHandleLatestEvents()
        {
            var latestEvents = _eventStore.GetEvents(_lastReadEventId);
            foreach(var evt in latestEvents)
            {
                var key = evt.GetType().Name;
                if (_eventHandlers.ContainsKey(key))
                    (_eventHandlers[key] as dynamic)((dynamic)evt);
            }
        }

        public void RegisterEventHandler<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            var key = typeof(TEvent).Name;
            _eventHandlers.Add(key, handler);
        }
    }
}
