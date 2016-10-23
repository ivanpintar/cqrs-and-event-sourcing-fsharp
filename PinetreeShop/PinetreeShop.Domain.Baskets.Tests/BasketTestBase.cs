using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.Domain.Tests;

namespace PinetreeShop.Domain.Baskets.Tests
{
    public class BasketTestBase : AggregateTestBase<BasketAggregate>
    {
        protected override ICommandDispatcher BuildCommandDispatcher()
        {
            _eventStore.AddPreviousEvents<BasketAggregate>(_preConditions);
            _aggregateRepository = new AggregateRepository(_eventStore);
            var commandDispatcher = new BasketCommandDispatcher(_aggregateRepository);
            
            return commandDispatcher;
        }
    }
}
