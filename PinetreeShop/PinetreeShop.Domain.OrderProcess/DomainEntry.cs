using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.CQRS.Infrastructure;
using System;

namespace PinetreeShop.Domain.OrderProcess
{
    public class DomainEntry : IDomainEntry
    {
        private IAggregateRepository _aggregateRepository;
        private IProcessManagerRepository _processManagerRepository;
        private ICommandDispatcher _commandDispatcher;
        private IEventListener _eventListener;

        public DomainEntry(
            IEventListener eventListener,
            IProcessManagerRepository processManagerRepository)
        {
            _eventListener = eventListener;
            _processManagerRepository = processManagerRepository;
            InitializeEventListener();
        }

        private void InitializeEventListener()
        {
            _eventListener.RegisterHandler(EventHandlers.BasketCheckedOut);
            _eventListener.RegisterHandler(EventHandlers.ProductReserved);
            _eventListener.RegisterHandler(EventHandlers.ProductReservationFailed);
            _eventListener.RegisterHandler(EventHandlers.OrderCreated);
            _eventListener.RegisterHandler(EventHandlers.OrderCancelled);
            _eventListener.RegisterHandler(EventHandlers.CreateOrderFailed);
            _eventListener.RegisterHandler(EventHandlers.OrderDelivered);
            _eventListener.RegisterHandler(EventHandlers.OrderShipped);
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
            _eventListener.HandleEvent<TEvent, TProcessManager>(evt);
        }
    }
}
