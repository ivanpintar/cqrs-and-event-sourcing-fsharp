using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PinetreeShop.CQRS.Infrastructure.Tests
{
    public class TestCommandDispatcher : ICommandDispatcher
    {
        public Action<ICommand> Action;

        public void ExecuteCommand<TAggregate>(ICommand command) where TAggregate : IAggregate, new()
        {
            Action(command);
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
                }
            };

            var commandListener = new CommandQueueListener<TestAggregate>(_eventStore, commandDispatcher);

            _eventStore.DispatchCommands(typeof(TestAggregate).Name, new List<ICommand> { new TestCommand(cmdGuid) });
            commandListener.DequeueAndDispatchCommand();

            Assert.True(state == 2);
        }
    }
}
