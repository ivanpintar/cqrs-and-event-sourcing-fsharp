using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.Domain.Tests;

namespace PinetreeShop.Domain.Products.Tests
{
    public class ProductTestBase : AggregateTestBase<ProductAggregate>
    {
        protected override IDomainEntry BuildApplication()
        {
            _eventStore.AddPreviousEvents<ProductAggregate>(_preConditions);
            _aggregateRepository = new AggregateRepository(_eventStore);
            var commandDispatcher = new CommandDispatcher(_aggregateRepository);

            return new DomainEntry(commandDispatcher, _aggregateRepository);
        }
    }
}
