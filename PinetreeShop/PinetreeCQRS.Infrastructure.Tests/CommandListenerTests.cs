using PinetreeCQRS.Infrastructure.Commands;
using System;
using System.Collections.Generic;
using Xunit;

namespace PinetreeCQRS.Infrastructure.Tests
{
    public class TestCommandDispatcher : ICommandDispatcher
    {
        public Func<ICommand, IAggregate> Action;

        public TAggregate ExecuteCommand<TAggregate>(ICommand command) where TAggregate : IAggregate, new()
        {
            return (TAggregate)Action(command);
        }

        public void RegisterHandler<TCommand, TAggregate>(Func<TAggregate, TCommand, TAggregate> handler)
            where TCommand : ICommand
            where TAggregate : IAggregate, new()
        {
            throw new NotImplementedException();
        }
    }

    public class TestCommand : CommandBase
    {
        public TestCommand(Guid aggregateId) : base(aggregateId)
        {
        }
    }

    public class TestAggregate : AggregateBase { }

    public class CommandListenerTests
    {
        IEventStore _eventStore = new TestEventStore();
        ICommandQueue _commandQueue = new TestCommandQueue();

        [Fact]
        public void When_CommandIsQueued_CommandDispatcherIsCalled()
        {
            var cmdGuid = Guid.NewGuid();
            var state = 0;
            var commandDispatcher = new TestCommandDispatcher()
            {
                Action = (cmd) =>
                {
                    if (cmd.AggregateId == cmdGuid)
                    {
                        state = 2;
                    }
                    return null;
                }
            };

            var commandListener = new CommandQueueListener<TestAggregate>(_eventStore, _commandQueue, commandDispatcher);

            _commandQueue.DispatchCommands(typeof(TestAggregate).Name, new List<ICommand> { new TestCommand(cmdGuid) });
            commandListener.DequeueAndDispatchCommands();

            Assert.True(state == 2);
        }
    }
}
