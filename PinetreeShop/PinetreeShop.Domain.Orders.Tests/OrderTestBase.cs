using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.Domain.Tests;

namespace PinetreeShop.Domain.Orders.Tests
{
    public class OrderTestBase : AggregateTestBase<OrderAggregate>
    {
        protected override ICommandDispatcher BuildCommandDispatcher()
        {
            _eventStore.AddPreviousEvents<OrderAggregate>(_preConditions);
            _aggregateRepository = new AggregateRepository(_eventStore);
            var commandDispatcher = new OrderCommandDispatcher(_aggregateRepository);

            return commandDispatcher;
        }
    }
}
