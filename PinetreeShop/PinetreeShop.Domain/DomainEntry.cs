using System;
using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.Domain.Baskets;
using PinetreeShop.Domain.Baskets.Commands;
using PinetreeShop.Domain.Orders;
using PinetreeShop.Domain.Orders.Commands;
using PinetreeShop.Domain.Products;
using PinetreeShop.Domain.Products.Commands;
using PinetreeShop.Domain.OrderProcess;
using PinetreeShop.Domain.Baskets.Events;
using PinetreeShop.Domain.Products.Events;

namespace PinetreeShop.Domain
{
    public class DomainEntry
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
            var productCommandHandler = new ProductCommandHandler(_aggregateRepository);
            _commandDispatcher.RegisterHandler<CreateProduct>(productCommandHandler);
            _commandDispatcher.RegisterHandler<ChangeProductQuantity>(productCommandHandler);
            _commandDispatcher.RegisterHandler<ReserveProduct>(productCommandHandler);
            _commandDispatcher.RegisterHandler<CancelProductReservation>(productCommandHandler);

            var basketCommandHandler = new BasketCommandHandler(_aggregateRepository);
            _commandDispatcher.RegisterHandler<CreateBasket>(basketCommandHandler);
            _commandDispatcher.RegisterHandler<AddItemToBasket>(basketCommandHandler);
            _commandDispatcher.RegisterHandler<RemoveItemFromBasket>(basketCommandHandler);
            _commandDispatcher.RegisterHandler<CancelBasket>(basketCommandHandler);
            _commandDispatcher.RegisterHandler<CheckOutBasket>(basketCommandHandler);

            var orderCommandHandler = new OrderCommandHandler(_aggregateRepository);
            _commandDispatcher.RegisterHandler<CreateOrder>(orderCommandHandler);
            _commandDispatcher.RegisterHandler<CancelOrder>(orderCommandHandler);
            _commandDispatcher.RegisterHandler<ShipOrder>(orderCommandHandler);
            _commandDispatcher.RegisterHandler<DeliverOrder>(orderCommandHandler);
        }

        private void InitializeEventListener()
        {
            var orderProcessEventHandler = new OrderProcessEventHandler(_processManagerRepository);
            _eventListener.RegisterHandler<BasketCheckedOut>(orderProcessEventHandler);
            _eventListener.RegisterHandler<ProductReserved>(orderProcessEventHandler);
        }
    }
}
