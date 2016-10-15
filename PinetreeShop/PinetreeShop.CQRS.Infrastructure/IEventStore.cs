using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;
using System;
using System.Collections.Generic;

namespace PinetreeShop.CQRS.Infrastructure
{
    public interface IEventStore
    {
        IEnumerable<IEvent> GetEvents(string category, Guid id, int startingPoint);
        IEnumerable<IEvent> GetEvents(string category, int startingPoint);
        void CommitEvents<TAggregate>(IEnumerable<IEvent> events);

        IEnumerable<ICommand> DeQueueCommands(string queueName);
        void DispatchCommands(string queueName, IEnumerable<ICommand> commands);
    }
}
