using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.CQRS.Persistence;
using PinetreeShop.Domain.Tests;

namespace PinetreeShop.Domain.Orders.Tests
{
    public class OrderTestBase : TestBase
    {
        protected override IDomainEntry BuildApplication()
        {
            _eventStore.AddPreviousEvents(_preConditions);
            _aggregateRepository = new AggregateRepository(_eventStore);
            var commandDispatcher = new CommandDispatcher(_aggregateRepository);
            _processManagerRepository = new ProcessManagerRepository(_eventStore);
            var eventListener = new EventListener(_processManagerRepository);

            return new DomainEntry(commandDispatcher, eventListener, _aggregateRepository, _processManagerRepository);
        }
    }
}
