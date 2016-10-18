using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.Domain.Tests;

namespace PinetreeShop.Domain.Baskets.Tests
{
    public class BasketTestBase : AggregateTestBase<BasketAggregate>
    {
        protected override IDomainEntry BuildApplication()
        {
            _eventStore.AddPreviousEvents<BasketAggregate>(_preConditions);
            _aggregateRepository = new AggregateRepository(_eventStore);
            var commandDispatcher = new CommandDispatcher(_aggregateRepository);

            return new DomainEntry(commandDispatcher, _aggregateRepository);
        }
    }
}
