using PinetreeShop.CQRS.Infrastructure.Events;
using System;
using System.Collections.Generic;

namespace PinetreeShop.CQRS.Infrastructure
{
    public interface IAggregate
    {
        Guid AggregateId { get; }
        int Version { get; }
        IEnumerable<IEvent> UncommittedEvents { get; }
        void ClearUncommittedEvents();
        void ApplyEvent(IEvent evt);
    }
}
