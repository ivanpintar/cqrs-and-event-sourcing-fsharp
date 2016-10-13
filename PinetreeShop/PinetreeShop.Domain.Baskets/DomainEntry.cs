using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.Domain.Baskets.Commands;

namespace PinetreeShop.Domain.Baskets
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
            var basketCommandHandler = new BasketCommandHandler(_aggregateRepository);
            _commandDispatcher.RegisterHandler<CreateBasket>(basketCommandHandler);
            _commandDispatcher.RegisterHandler<AddItemToBasket>(basketCommandHandler);
            _commandDispatcher.RegisterHandler<RemoveItemFromBasket>(basketCommandHandler);
            _commandDispatcher.RegisterHandler<CancelBasket>(basketCommandHandler);
            _commandDispatcher.RegisterHandler<CheckOutBasket>(basketCommandHandler);
        }

        private void InitializeEventListener()
        {            
        }
    }
}
