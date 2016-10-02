using System;

namespace PinetreeShop.CQRS.Infrastructure
{
    public interface IEvent
    {
        Guid AggregateId { get; }
        DateTime Date { get; }
    }

    public class EventBase : IEvent
    {
        public Guid AggregateId { get; set; }
        public DateTime Date { get; private set; }

        public EventBase(Guid aggregateId)
        {
            AggregateId = aggregateId;
            Date = DateTime.Now;
        }
    }
}
