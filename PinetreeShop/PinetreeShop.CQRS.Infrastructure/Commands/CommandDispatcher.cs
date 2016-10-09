using PinetreeShop.CQRS.Infrastructure.Repositories;
using System;
using System.Collections.Generic;

namespace PinetreeShop.CQRS.Infrastructure.Commands
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private Dictionary<Type, Func<object, IAggregate>> _commandHandlers = new Dictionary<Type, Func<object, IAggregate>>();
        private IAggregateRepository _aggregateRepository;

        public CommandDispatcher(IAggregateRepository aggregateRepository)
        {
            _aggregateRepository = aggregateRepository;
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
            foreach(var evt in aggregate.UncommittedEvents)
            {
                evt.Metadata.CausationId = command.Metadata.CommandId;
                evt.Metadata.CorrelationId = command.Metadata.CorrelationId;
            }

            _aggregateRepository.SaveAggregate(aggregate);
        }
    }
}
