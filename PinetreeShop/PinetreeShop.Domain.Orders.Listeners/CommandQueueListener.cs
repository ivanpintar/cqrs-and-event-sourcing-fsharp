using PinetreeCQRS.Infrastructure.Commands;
using PinetreeCQRS.Infrastructure.Repositories;
using PinetreeCQRS.Persistence.SQL;

namespace PinetreeShop.Domain.Orders.Listeners
{
    public class CommandQueueListener
    {
        private CommandQueueListener<OrderAggregate> _commandQueueListener;

        public CommandQueueListener()
        {
            var eventStore = new SqlEventStore();
            var commandDispatcher = new OrderCommandDispatcher(new AggregateRepository(eventStore));
            _commandQueueListener = new CommandQueueListener<OrderAggregate>(eventStore, eventStore, commandDispatcher);
        }

        public void ProcessCommands()
        {
            _commandQueueListener.DequeueAndDispatchCommands();
        }
    }
}
