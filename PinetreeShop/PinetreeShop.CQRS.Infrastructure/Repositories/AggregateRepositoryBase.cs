using PinetreeShop.CQRS.Infrastructure.Events;
using System;
using System.Collections.Generic;

namespace PinetreeShop.CQRS.Infrastructure.Repositories
{
    public abstract class AggregateRepositoryBase : IAggregateRepository
    {       
        protected int CalculateExpectedVersion<T>(IAggregate aggregate, List<T> events)
        {
            return aggregate.Version - events.Count;
        }

        protected TResult BuildAggregate<TResult>(IEnumerable<IEvent> events) where TResult : IAggregate, new()
        {
            var result = new TResult();
            foreach(var evt in events)
            {
                result.ApplyEvent(evt);
            }

            result.ClearUncommittedEvents();
            return result;
        }

        public abstract void SaveAggregate<TAggregate>(TAggregate aggregate) where TAggregate : IAggregate;
        public abstract TResult GetAggregateById<TResult>(Guid id) where TResult : IAggregate, new();
    }
}
