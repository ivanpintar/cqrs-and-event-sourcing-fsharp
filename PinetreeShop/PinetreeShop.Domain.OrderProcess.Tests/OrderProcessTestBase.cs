using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.CQRS.Persistence;
using PinetreeShop.Domain.Tests;

namespace PinetreeShop.Domain.OrderProcess.Tests
{
    public class OrderProcessTestBase : ProcessManagerTestBase<OrderProcessManager>
    {
        protected override IDomainEntry BuildApplication()
        {
            _eventStore.AddPreviousEvents(_preConditions);
            _processManagerRepository = new ProcessManagerRepository(_eventStore);
            var eventListener = new EventListener(_processManagerRepository);

            return new DomainEntry(eventListener, _processManagerRepository);
        }
    }
}
