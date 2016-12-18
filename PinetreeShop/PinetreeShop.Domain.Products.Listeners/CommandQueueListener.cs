using PinetreeCQRS.Infrastructure.Commands;
using PinetreeCQRS.Infrastructure.Repositories;
using PinetreeCQRS.Persistence.SQL;

namespace PinetreeShop.Domain.Products.Listeners
{
    public class CommandQueueListener
    {
        private CommandQueueListener<ProductAggregate> _commandQueueListener;

        public CommandQueueListener()
        {
            var eventStore =  new SqlEventStore();
            var commandDispatcher = new ProductCommandDispatcher(new AggregateRepository(eventStore));
            _commandQueueListener = new CommandQueueListener<ProductAggregate>(eventStore, eventStore, commandDispatcher);
        }

        public void ProcessCommands()
        {
            _commandQueueListener.DequeueAndDispatchCommands();
        }
    }
}
