using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.CQRS.Infrastructure;
using System;

namespace PinetreeShop.Domain.OrderProcess
{
    public class DomainEntry : IDomainEntry
    {
        private IProcessManagerRepository _processManagerRepository;
        private IEventHandler _eventHandler;

        public DomainEntry(
            IEventHandler eventHandler,
            IProcessManagerRepository processManagerRepository)
        {
            _eventHandler = eventHandler;
            _processManagerRepository = processManagerRepository;
            InitializeEventListener();
        }

        private void InitializeEventListener()
        {
            _eventHandler.RegisterHandler(EventHandlers.BasketCheckedOut);
            _eventHandler.RegisterHandler(EventHandlers.ProductReserved);
            _eventHandler.RegisterHandler(EventHandlers.ProductReservationFailed);
            _eventHandler.RegisterHandler(EventHandlers.OrderCreated);
            _eventHandler.RegisterHandler(EventHandlers.OrderCancelled);
            _eventHandler.RegisterHandler(EventHandlers.CreateOrderFailed);
            _eventHandler.RegisterHandler(EventHandlers.OrderDelivered);
            _eventHandler.RegisterHandler(EventHandlers.OrderShipped);
        }

        public void ExecuteCommand<TCommand, TAggregate>(TCommand command)
            where TCommand : ICommand
            where TAggregate : IAggregate, new()
        {
            throw new NotImplementedException();
        }

        public void HandleEvent<TEvent, TProcessManager>(TEvent evt)
            where TEvent : IEvent
            where TProcessManager : IProcessManager, new()
        {
            _eventHandler.HandleEvent<TEvent, TProcessManager>(evt);
        }
    }
}
