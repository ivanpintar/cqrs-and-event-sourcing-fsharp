using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.Domain.Tests;

namespace PinetreeShop.Domain.OrderProcess.Tests
{
    public class OrderProcessTestBase : ProcessManagerTestBase<OrderProcessManager>
    {
        protected override IDomainEntry BuildApplication()
        {
            _eventStore.AddPreviousEvents(_preConditions);
            _processManagerRepository = new ProcessManagerRepository(_eventStore);
            var eventHandler = new ProcessEventHandler(_processManagerRepository);

            return new DomainEntry(eventHandler, _processManagerRepository);
        }
    }
}
