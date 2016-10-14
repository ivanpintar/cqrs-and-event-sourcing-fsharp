using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.CQRS.Persistence;
using PinetreeShop.Domain.Tests;

namespace PinetreeShop.Domain.Products.Tests
{
    public class ProductTestBase : AggregateTestBase<ProductAggregate>
    {
        protected override IDomainEntry BuildApplication()
        {
            _eventStore.AddPreviousEvents(_preConditions);
            _aggregateRepository = new AggregateRepository(_eventStore);
            var commandDispatcher = new CommandDispatcher(_aggregateRepository);

            return new DomainEntry(commandDispatcher, _aggregateRepository);
        }
    }
}
