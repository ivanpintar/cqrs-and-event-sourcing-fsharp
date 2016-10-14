using System;
using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.Domain.Baskets.Commands;

namespace PinetreeShop.Domain.Baskets
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
            _commandDispatcher.ExecuteCommand<TCommand, TAggregate>(command);
        }

        private void InitializeCommandDispatcher()
        {
            _commandDispatcher.RegisterHandler(CommandHandler.Create);
            _commandDispatcher.RegisterHandler(CommandHandler.RemoveItem);
            _commandDispatcher.RegisterHandler(CommandHandler.CheckOut);
            _commandDispatcher.RegisterHandler(CommandHandler.AddItem);
            _commandDispatcher.RegisterHandler(CommandHandler.Cancel);
        }

        public void HandleEvent<TEvent, TProcessManager>(TEvent evt)
            where TEvent : IEvent
            where TProcessManager : IProcessManager, new()
        {
            throw new NotImplementedException();
        }
    }
}
