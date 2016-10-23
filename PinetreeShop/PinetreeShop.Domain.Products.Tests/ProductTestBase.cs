using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.Domain.Tests;

namespace PinetreeShop.Domain.Products.Tests
{
    public class ProductTestBase : AggregateTestBase<ProductAggregate>
    {
        protected override ICommandDispatcher BuildCommandDispatcher()
        {
            _eventStore.AddPreviousEvents<ProductAggregate>(_preConditions);
            _aggregateRepository = new AggregateRepository(_eventStore);
            return new ProductCommandDispatcher(_aggregateRepository);
        }
    }
}
