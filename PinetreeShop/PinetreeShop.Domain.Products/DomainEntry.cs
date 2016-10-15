using System;
using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.Domain.Products.Commands;

namespace PinetreeShop.Domain.Products
{
    public class DomainEntry : IDomainEntry
    {
        private IAggregateRepository _aggregateRepository;
        private ICommandDispatcher _commandDispatcher;
        
        public DomainEntry(
            ICommandDispatcher commandDispatcher, 
            IAggregateRepository aggregateRepository)
        {
            _commandDispatcher = commandDispatcher;
            _aggregateRepository = aggregateRepository;
            InitializeCommandDispatcher();
        }

        public void ExecuteCommand<TCommand, TAggregate>(TCommand command)
            where TCommand : ICommand
            where TAggregate : IAggregate, new()
        {
            _commandDispatcher.ExecuteCommand<ProductAggregate>(command);
        }
        
        private void InitializeCommandDispatcher()
        {
            _commandDispatcher.RegisterHandler(CommandHandlers.Create);
            _commandDispatcher.RegisterHandler(CommandHandlers.ChangeQuantity);
            _commandDispatcher.RegisterHandler(CommandHandlers.Reserve);
            _commandDispatcher.RegisterHandler(CommandHandlers.CancelReservation);           
        }

        public void HandleEvent<TEvent, TProcessManager>(TEvent evt)
            where TEvent : IEvent
            where TProcessManager : IProcessManager, new()
        {
            throw new NotImplementedException();
        }
    }
}
