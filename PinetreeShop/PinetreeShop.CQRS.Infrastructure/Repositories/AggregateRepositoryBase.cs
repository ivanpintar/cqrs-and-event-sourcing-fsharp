using PinetreeShop.CQRS.Infrastructure.Events;
using System;
using System.Collections.Generic;

namespace PinetreeShop.CQRS.Infrastructure.Repositories
{
    public abstract class AggregateRepositoryBase : IAggregateRepository
    {
        public static Func<Guid> CreateGuid = () => Guid.NewGuid();

        protected int CalculateExpectedVersion<T>(IAggregate aggregate, List<T> events)
        {
            return aggregate.Version - events.Count;
        }

        protected TAggregate BuildAggregate<TAggregate>(IEnumerable<IEvent> events) where TAggregate : IAggregate, new()
        {
            var result = new TAggregate();
            foreach(var evt in events)
            {
                result.ApplyEvent(evt);
            }
            return result;
        }

        public abstract void SaveAggregate<TAggregate>(TAggregate aggregate) where TAggregate : IAggregate;
        public abstract TAggregate GetAggregateById<TAggregate>(Guid id) where TAggregate : IAggregate, new();
    }
}
