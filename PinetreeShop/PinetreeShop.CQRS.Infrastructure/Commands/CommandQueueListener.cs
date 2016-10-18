using System;
using System.Collections.Generic;
using System.Linq;

namespace PinetreeShop.CQRS.Infrastructure.Commands
{
    public class CommandQueueListener<TAggregate> where TAggregate : class, IAggregate, new()
    {
        private ICommandDispatcher _commandDispatcher;
        private IEventStore _eventStore;
        private string _queueName;

        public CommandQueueListener(IEventStore eventStore, ICommandDispatcher commandDispatcher)
        {
            _eventStore = eventStore;
            _commandDispatcher = commandDispatcher;
            _queueName = typeof(TAggregate).Name;
        }

        public void DequeueAndDispatchCommand()
        {
            var commands = _eventStore.DeQueueCommands(_queueName);
            foreach(var cmd in commands)
            {
                _commandDispatcher.ExecuteCommand<TAggregate>(cmd);
            }
        }
    }
}
