using PinetreeCQRS.Infrastructure.Events;
using PinetreeCQRS.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PinetreeCQRS.Infrastructure.Repositories
{
    public class AggregateRepository : IAggregateRepository
    {
        public static Func<Guid> CreateGuid = () => Guid.NewGuid();

        private IEventStore _eventStore;

        public AggregateRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public TAggregate GetAggregateById<TAggregate>(Guid id) where TAggregate : IAggregate, new()
        {
            var events = GetEventsForAggregate<TAggregate>(id);
            if (events.Any())
            {
                return BuildAggregate<TAggregate>(events);
            }

            return default(TAggregate);
        }

        public void SaveAggregate<TAggregate>(TAggregate aggregate) where TAggregate : IAggregate
        {
            var eventsToSave = aggregate.UncommittedEvents.ToList();
            var expectedVersion = CalculateExpectedVersion(aggregate, eventsToSave);

            if (expectedVersion >= 0)
            {
                var existingEvents = GetEventsForAggregate<TAggregate>(aggregate.AggregateId);
                var currentversion = existingEvents.Count - 1;
                if (currentversion != expectedVersion)
                {
                    throw new WrongExpectedVersionException($"{aggregate.GetType()}:{aggregate.AggregateId}: Expected version {expectedVersion} but the version is {currentversion}");
                }
            }

            _eventStore.CommitEvents<TAggregate>(eventsToSave);
            aggregate.ClearUncommittedEvents();
        }
        
        private List<IEvent> GetEventsForAggregate<TAggregate>(Guid aggregateId)
        {
            return _eventStore.GetAggregateEvents<TAggregate>(aggregateId, 0).ToList();                
        }


        private int CalculateExpectedVersion<T>(IAggregate aggregate, List<T> events)
        {
            return aggregate.Version - events.Count;
        }

        private TAggregate BuildAggregate<TAggregate>(IEnumerable<IEvent> events) where TAggregate : IAggregate, new()
        {
            var result = new TAggregate();
            foreach (var evt in events)
            {
                result.ApplyEvent(evt);
            }
            return result;
        }
    }
}
