using System;
using System.Collections.Generic;
using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.CQRS.Infrastructure;

namespace PinetreeShop.CQRS.Persistence.SQL
{
    public class SqlEventStore : IEventStore
    {
        public void CommitEvents<TAggregate>(IEnumerable<IEvent> events)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICommand> DeQueueCommands(string queueName)
        {
            throw new NotImplementedException();
        }

        public void DispatchCommands(string queueName, IEnumerable<ICommand> commands)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEvent> GetEvents(string category, int startingPoint)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEvent> GetEvents(string category, Guid id, int startingPoint)
        {
            throw new NotImplementedException();
        }

        private string GetStreamName<TAggregate>(Guid id)
        {
            return $"{typeof(TAggregate).Name}-{id}";
        }
    }
}
