using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.Domain.Orders.Commands;

namespace PinetreeShop.Domain.Orders
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
            var orderCommandHandler = new OrderCommandHandler(_aggregateRepository);
            _commandDispatcher.RegisterHandler<CreateOrder>(orderCommandHandler);
            _commandDispatcher.RegisterHandler<CancelOrder>(orderCommandHandler);
            _commandDispatcher.RegisterHandler<ShipOrder>(orderCommandHandler);
            _commandDispatcher.RegisterHandler<DeliverOrder>(orderCommandHandler);
        }

        private void InitializeEventListener()
        {
        }
    }
}
