using System;

namespace PinetreeShop.CQRS.Infrastructure.CommandsAndEvents
{
    public interface IEvent
    {
        Guid AggregateId { get; }
        DateTime Date { get; }
    }

    public class EventBase : IEvent
    {
        public Guid AggregateId { get; private set; }
        public DateTime Date { get; private set; }

        public EventBase(Guid aggregateId)
        {
            AggregateId = aggregateId;
            Date = DateTime.Now;
        }
    }

    public class EventFailedBase : IEvent
    {
        public static string UnknownError = "UnknownError";

        public Guid AggregateId { get; private set; }
        public DateTime Date { get; private set; }
        public string Reason { get; private set; }

        public EventFailedBase(Guid aggregateId, string reason)
        {
            AggregateId = aggregateId;
            Date = DateTime.Now;
            Reason = reason;
        }
    }
}
