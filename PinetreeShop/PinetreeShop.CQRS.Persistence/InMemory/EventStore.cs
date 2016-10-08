using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.CQRS.Persistence.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinetreeShop.CQRS.Persistence.InMemory
{
    public class TestEventStore : IEventStore
    {
        private List<IEvent> _events = new List<IEvent>();
        private List<IEvent> _latestEvents;

        public IEnumerable<IEvent> Events { get { return _events; } }

        public IEnumerable<IEvent> GetLatestEvents()
        {
            return _latestEvents;
        }

        public void CommitEvents(IEnumerable<IEvent> events)
        {
            _events.AddRange(events);
            _latestEvents = events.ToList();
        }

        public void AddPreviousEvents(IEnumerable<IEvent> events)
        {
            _events.AddRange(events);
        }
    }
}
