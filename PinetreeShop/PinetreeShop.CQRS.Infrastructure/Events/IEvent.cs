using System;

namespace PinetreeShop.CQRS.Infrastructure.Events
{
    public interface IEvent
    {
        Guid AggregateId { get; }
        DateTime Date { get; }
    }

    public class EventBase : IEvent
    {
        public Guid EventId { get; private set; }
        public Guid AggregateId { get; set; }
        public DateTime Date { get; set; }

        public EventBase(Guid aggregateId)
        {
            EventId = Guid.NewGuid();
            AggregateId = aggregateId;
            Date = DateTime.Now;
        }
    }

    public class EventFailedBase : EventBase
    {
        public static string UnknownError = "UnknownError";        
        public string Reason { get; set; }

        public EventFailedBase(Guid aggregateId, string reason) : base(aggregateId)
        {
            Reason = reason;
        }
    }
}
