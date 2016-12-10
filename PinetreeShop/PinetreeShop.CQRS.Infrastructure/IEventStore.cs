using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;
using System;
using System.Collections.Generic;

namespace PinetreeShop.CQRS.Infrastructure
{
    public interface IEventStore
    {
        IEnumerable<IEvent> GetEvents(int startingPoint);
        IEnumerable<IEvent> GetEvents<TAggregate>(int lastEventNumber);
        IEnumerable<IEvent> GetAggregateEvents<TAggregate>(Guid aggregateId, int lastEventNumber);
        void CommitEvents<TAggregate>(IEnumerable<IEvent> events);
    }
}
