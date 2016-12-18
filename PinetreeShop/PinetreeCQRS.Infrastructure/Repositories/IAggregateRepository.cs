using System;

namespace PinetreeCQRS.Infrastructure.Repositories
{
    public interface IAggregateRepository
    {
        void SaveAggregate<TAggregate>(TAggregate aggregate) where TAggregate : IAggregate;
        TResult GetAggregateById<TResult>(Guid id) where TResult : IAggregate, new();
    }
}
