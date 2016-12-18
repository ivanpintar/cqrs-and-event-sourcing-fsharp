using PinetreeCQRS.Infrastructure.Events;
using System;
using System.Collections.Generic;

namespace PinetreeCQRS.Infrastructure.Commands
{
    public class CommandQueueListener<TAggregate> where TAggregate : class, IAggregate, new()
    {
        private ICommandDispatcher _commandDispatcher;
        private IEventStore _eventStore;
        private ICommandQueue _commandQueue;
        private string _queueName;

        public CommandQueueListener(IEventStore eventStore, ICommandQueue commandQueue, ICommandDispatcher commandDispatcher)
        {
            _eventStore = eventStore;
            _commandQueue = commandQueue;
            _commandDispatcher = commandDispatcher;
            _queueName = typeof(TAggregate).Name;
        }

        public void DequeueAndDispatchCommands()
        {
            var commands = _commandQueue.DeQueueCommands(_queueName);
            foreach(var cmd in commands)
            {
                try
                {
                    _commandDispatcher.ExecuteCommand<TAggregate>(cmd);
                }
                catch (Exception ex)
                {
                    var failureEvent = new EventFailedBase(cmd.AggregateId, ex.Message);
                    failureEvent.Metadata.CausationId = cmd.Metadata.CausationId;
                    failureEvent.Metadata.CorrelationId = cmd.Metadata.CorrelationId;
                    _eventStore.CommitEvents<TAggregate>(new List<IEvent> { failureEvent });
                }
            }
        }
    }
}
