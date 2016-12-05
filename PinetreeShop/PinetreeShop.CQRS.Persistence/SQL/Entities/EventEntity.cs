using System;

namespace PinetreeShop.CQRS.Persistence.SQL.Entities
{
    public class EventEntity
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public Guid AggregateId { get; set; }
        public Guid EventId { get; set; }
        public Guid CausationId { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid ProcessId { get; set; }

        public string EventPayload { get; set; }
    }
}
