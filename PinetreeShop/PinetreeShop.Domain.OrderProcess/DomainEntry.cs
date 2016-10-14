using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.Domain.Baskets.Events;
using PinetreeShop.Domain.Products.Events;
using PinetreeShop.Domain.Orders.Events;

namespace PinetreeShop.Domain.OrderProcess
{
    public class DomainEntry : IDomainEntry
    {
        private IAggregateRepository _aggregateRepository;
        private IProcessManagerRepository _processManagerRepository;
        private ICommandDispatcher _commandDispatcher;
        private IEventListener _eventListener;

        public DomainEntry(
            ICommandDispatcher commandDispatcher, 
            IEventListener eventListener,
            IAggregateRepository aggregateRepository, 
            IProcessManagerRepository processManagerRepository)
        {
            _commandDispatcher = commandDispatcher;
            _eventListener = eventListener;
            _aggregateRepository = aggregateRepository;
            _processManagerRepository = processManagerRepository;
            InitializeCommandDispatcher();
            InitializeEventListener();
        }

        public void ExecuteCommand<TCommand>(TCommand command) where TCommand : ICommand
        {
            _commandDispatcher.ExecuteCommand(command);
        }

        public void HandleEvent<TEvent>(TEvent evt) where TEvent : IEvent
        {
            _eventListener.HandleEvent(evt);
        }

        private void InitializeCommandDispatcher()
        {
        }

        private void InitializeEventListener()
        {
            var orderProcessEventHandler = new OrderProcessEventHandler(_processManagerRepository);
            _eventListener.RegisterHandler<BasketCheckedOut>(orderProcessEventHandler);
            _eventListener.RegisterHandler<ProductReserved>(orderProcessEventHandler);
            _eventListener.RegisterHandler<ProductReservationFailed>(orderProcessEventHandler);
            _eventListener.RegisterHandler<OrderCreated>(orderProcessEventHandler);
            _eventListener.RegisterHandler<CreateOrderFailed>(orderProcessEventHandler);
            _eventListener.RegisterHandler<OrderCancelled>(orderProcessEventHandler);
            _eventListener.RegisterHandler<OrderShipped>(orderProcessEventHandler);
            _eventListener.RegisterHandler<OrderDelivered>(orderProcessEventHandler);
        }
    }
}
