using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;
using System.Collections.Generic;

namespace PinetreeShop.CQRS.Persistence.Exceptions
{
    public interface IEventStore
    {
        IEnumerable<IEvent> Events { get; }
        void CommitEvents(IEnumerable<IEvent> events);
    }
}
