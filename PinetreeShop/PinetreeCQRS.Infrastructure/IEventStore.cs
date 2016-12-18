using PinetreeCQRS.Infrastructure.Commands;
using PinetreeCQRS.Infrastructure.Events;
using System;
using System.Collections.Generic;

namespace PinetreeCQRS.Infrastructure
{
    public interface IEventStore
    {
        IEnumerable<IEvent> GetEvents(int startingPoint);
        IEnumerable<IEvent> GetEvents<TAggregate>(int lastEventNumber);
        IEnumerable<IEvent> GetAggregateEvents<TAggregate>(Guid aggregateId, int lastEventNumber);
        void CommitEvents<TAggregate>(IEnumerable<IEvent> events);
    }
}
