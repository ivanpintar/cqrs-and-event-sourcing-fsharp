using System;
using System.Collections.Generic;

namespace PinetreeShop.CQRS.Infrastructure
{
    public interface IAggregate
    {
        Guid Id { get; }
        int Version { get; }
        IEnumerable<IEvent> UncommittedEvents { get; }
        void ClearUncommittedEvents();
        void ApplyEvent(IEvent evt);
    }
}
