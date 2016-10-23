using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.Domain.Tests;

namespace PinetreeShop.Domain.OrderProcess.Tests
{
    public class OrderProcessTestBase : ProcessManagerTestBase<OrderProcessManager>
    {
        protected override IProcessEventHandler BuildApplication()
        {
            _eventStore.AddPreviousEvents(_preConditions);
            _processManagerRepository = new ProcessManagerRepository(_eventStore);
            var eventHandler = new ProcessEventHandler(_processManagerRepository);

            DomainEntry.InitializeEventHandler(eventHandler);

            return eventHandler;
        }
    }
}
