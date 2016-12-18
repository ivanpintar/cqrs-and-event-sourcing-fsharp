using PinetreeCQRS.Infrastructure.Events;
using PinetreeCQRS.Infrastructure.Repositories;
using PinetreeShop.Domain.Tests;

namespace PinetreeShop.Domain.OrderProcess.Tests
{
    public class OrderProcessTestBase : ProcessManagerTestBase<OrderProcessManager>
    {
        protected override IProcessEventHandler BuildApplication()
        {
            _eventStore.AddPreviousEvents(_preConditions);
            _processManagerRepository = new ProcessManagerRepository(_eventStore, _commandQueue);
            return new OrderProcessEventHandler(_processManagerRepository);
        }
    }
}
