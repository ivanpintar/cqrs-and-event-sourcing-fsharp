using PinetreeShop.CQRS.Infrastructure.Repositories;
using System;
using System.Collections.Generic;

namespace PinetreeShop.CQRS.Infrastructure.Commands
{
    public class CommandDispatcher
    {
        private Dictionary<Type, Func<object, IAggregate>> _commandHandlers = new Dictionary<Type, Func<object, IAggregate>>();
        private IAggregateRepository _domainRepository;

        public CommandDispatcher(IAggregateRepository aggregateRepository)
        {
            _domainRepository = aggregateRepository;
        }

        public void RegisterHandler<TCommand>(IHandleCommand<TCommand> handler) where TCommand : class, ICommand
        {
            _commandHandlers.Add(typeof(TCommand), command => handler.Handle(command as TCommand));
        }

        public void ExecuteCommand<TCommand>(TCommand command) where TCommand : ICommand
        {
            var commandType = command.GetType();

            if (!_commandHandlers.ContainsKey(commandType))
            {
                throw new ApplicationException($"Missing handler for {commandType.Name}");
            }

            var aggregate = _commandHandlers[commandType](command);
            _domainRepository.SaveAggregate(aggregate);
        }
    }
}
