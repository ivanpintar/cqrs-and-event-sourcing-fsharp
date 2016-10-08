using Newtonsoft.Json;
using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.CQRS.Persistence.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PinetreeShop.CQRS.Persistence
{
    public class InMemoryAggregateRepository : AggregateRepositoryBase
    {
        public List<IEvent> _eventStore = new List<IEvent>();
        private List<IEvent> _latestEvents = new List<IEvent>();
        private List<ICommand> _latestCommands = new List<ICommand>();

        public override TResult GetAggregateById<TResult>(Guid id)
        {
            var events = _eventStore.Where(e => e.AggregateId == id);
            if (events.Any())
            {
                return BuildAggregate<TResult>(events);
            }
            throw new AggregateNotFoundException($"Could not find aggregate {typeof(TResult)}:{id}");
        }

        public override void SaveAggregate<TAggregate>(TAggregate aggregate)
        {
            var eventsToSave = aggregate.UncommittedEvents.ToList();
            var expectedVersion = CalculateExpectedVersion(aggregate, eventsToSave);
            if (expectedVersion < 0)
            {
                _eventStore.AddRange(eventsToSave);
            }
            else
            {
                var existingEvents = GetEventsForAggregate(aggregate.AggregateId);
                var currentversion = existingEvents.Count - 1;
                if (currentversion != expectedVersion)
                {
                    throw new WrongExpectedVersionException($"{aggregate.GetType()}:{aggregate.AggregateId}: Expected version {expectedVersion} but the version is {currentversion}");
                }
                existingEvents.AddRange(eventsToSave);
            }
            _latestEvents.AddRange(eventsToSave);
            aggregate.ClearUncommittedEvents();
        }
       
        public IEnumerable<IEvent> GetLatestEvents()
        {
            return _latestEvents;
        }

        public void AddEvents(IEnumerable<IEvent> events)
        {
            _eventStore.AddRange(events);
        }

        private List<IEvent> GetEventsForAggregate(Guid aggregateId)
        {
            return _eventStore.Where(e => e.AggregateId == aggregateId).ToList();
        }
    }
}
