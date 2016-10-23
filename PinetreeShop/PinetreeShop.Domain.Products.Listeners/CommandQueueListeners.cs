using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.CQRS.Persistence.SQL;
using PinetreeShop.Domain.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinetreeShop.Domain.Products.Listeners
{
    public class CommandQueueListeners
    {
        private CommandQueueListener<ProductAggregate> _commandQueueListener;

        public CommandQueueListeners()
        {
            var eventStore =  new SqlEventStore();
            var commandDispatcher = new ProductCommandDispatcher(new AggregateRepository(eventStore));
            _commandQueueListener = new CommandQueueListener<ProductAggregate>(eventStore, commandDispatcher);
        }

        public void ProcessCommands()
        {
            _commandQueueListener.DequeueAndDispatchCommands();
        }
    }
}
