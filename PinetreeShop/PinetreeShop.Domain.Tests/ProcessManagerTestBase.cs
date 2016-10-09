using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.CQRS.Persistence;

namespace PinetreeShop.Domain.Tests
{
    public class ProcessManagerTestBase : TestBase
    {
        protected override DomainEntry BuildApplication()
        {
            _eventStore.AddPreviousEvents(_preConditions);
            _aggregateRepository = new AggregateRepository(_eventStore);
            var commandDispatcher = new MockCommandDispatcher();
            _processManagerRepository = new ProcessManagerRepository(_eventStore, commandDispatcher);
            var eventListener = new EventListener(_processManagerRepository);
            
            return new DomainEntry(commandDispatcher, eventListener, _aggregateRepository, _processManagerRepository);
        }
    }
}
