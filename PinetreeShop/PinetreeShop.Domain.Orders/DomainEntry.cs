using System;
using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.Domain.Orders.Commands;

namespace PinetreeShop.Domain.Orders
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
            _commandDispatcher.ExecuteCommand<OrderAggregate>(command);
        }

        public void HandleEvent<TEvent, TProcessManager>(TEvent evt)
            where TEvent : IEvent
            where TProcessManager : IProcessManager, new()
        {
            throw new NotImplementedException();
        }

        private void InitializeCommandDispatcher()
        {
            _commandDispatcher.RegisterHandler(CommandHandlers.Create);
            _commandDispatcher.RegisterHandler(CommandHandlers.Cancel);
            _commandDispatcher.RegisterHandler(CommandHandlers.Ship);
            _commandDispatcher.RegisterHandler(CommandHandlers.Deliver);
        }        
    }
}
