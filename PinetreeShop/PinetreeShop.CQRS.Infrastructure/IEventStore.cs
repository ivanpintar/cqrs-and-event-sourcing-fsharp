using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;
using System;
using System.Collections.Generic;

namespace PinetreeShop.CQRS.Infrastructure
{
    public interface IEventStore
    {
        IEnumerable<IEvent> GetEvents(int startingPoint);
        IEnumerable<IEvent> GetEvents<TAggregate>(int startingPoint) where TAggregate : IAggregate;
        IEnumerable<IEvent> GetAggregateEvents<TAggregate>(Guid aggregateId, int startingPoint) where TAggregate : IAggregate;
        IEnumerable<IEvent> GetProcessEvents(Guid correlatioId, int startingPoint);
        void CommitEvents<TAggregate>(IEnumerable<IEvent> events);

        IEnumerable<ICommand> DeQueueCommands(string queueName);
        void DispatchCommands(string queueName, IEnumerable<ICommand> commands);
    }
}
