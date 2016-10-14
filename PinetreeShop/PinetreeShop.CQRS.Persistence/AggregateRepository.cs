using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.CQRS.Persistence.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PinetreeShop.CQRS.Persistence
{
    public class AggregateRepository : AggregateRepositoryBase
    {
        private IEventStore _eventStore;

        public AggregateRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public override TResult GetAggregateById<TResult>(Guid id)
        {
            var events = GetEventsForAggregate(id);
            if (events.Any())
            {
                return BuildAggregate<TResult>(events);
            }

            return default(TResult);
        }

        public override void SaveAggregate<TAggregate>(TAggregate aggregate)
        {
            var eventsToSave = aggregate.UncommittedEvents.ToList();
            var expectedVersion = CalculateExpectedVersion(aggregate, eventsToSave);

            if (expectedVersion >= 0)
            {
                var existingEvents = GetEventsForAggregate(aggregate.AggregateId);
                var currentversion = existingEvents.Count - 1;
                if (currentversion != expectedVersion)
                {
                    throw new WrongExpectedVersionException($"{aggregate.GetType()}:{aggregate.AggregateId}: Expected version {expectedVersion} but the version is {currentversion}");
                }
            }

            _eventStore.CommitEvents(eventsToSave);
            aggregate.ClearUncommittedEvents();
        }

        private List<IEvent> GetEventsForAggregate(Guid aggregateId)
        {
            return _eventStore.Events.Where(e => e.AggregateId == aggregateId).ToList();
        }
    }
}
