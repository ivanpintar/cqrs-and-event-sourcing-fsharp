using System;
using System.Collections.Generic;

namespace PinetreeShop.CQRS.Infrastructure
{
    public abstract class DomainRepositoryBase : IDomainRepository
    {
        public abstract TResult GetById<TResult>(Guid id) where TResult : IAggregate, new();
        public abstract IEnumerable<IEvent> Save<TAggregate>(TAggregate aggregate) where TAggregate : IAggregate;

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

            return result;
        }
    }
}
