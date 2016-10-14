using PinetreeShop.CQRS.Infrastructure.Repositories;
using System;
using System.Collections.Generic;

namespace PinetreeShop.CQRS.Infrastructure.Commands
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private Dictionary<Type, object> _commandHandlers = new Dictionary<Type, object>();
        private IAggregateRepository _aggregateRepository;

        public CommandDispatcher(IAggregateRepository aggregateRepository)
        {
            _aggregateRepository = aggregateRepository;
        }

        public void RegisterHandler<TCommand, TAggregate>(Func<TAggregate, TCommand, TAggregate> handler)
            where TCommand : class, ICommand
            where TAggregate : IAggregate

        {
            _commandHandlers.Add(typeof(TCommand), handler);
        }

        public void ExecuteCommand<TCommand, TAggregate>(TCommand command)
            where TCommand : ICommand
            where TAggregate : IAggregate, new()
        {
            var commandType = command.GetType();

            if (!_commandHandlers.ContainsKey(commandType))
            {
                throw new ApplicationException($"Missing handler for {commandType.Name}");
            }

            var aggregate = _aggregateRepository.GetAggregateById<TAggregate>(command.AggregateId);

            aggregate = (_commandHandlers[commandType] as Func<TAggregate, TCommand, TAggregate>)(aggregate, command);

            foreach (var evt in aggregate.UncommittedEvents)
            {
                evt.Metadata.CausationId = command.Metadata.CommandId;
                evt.Metadata.CorrelationId = command.Metadata.CorrelationId;
            }

            _aggregateRepository.SaveAggregate(aggregate);
        }
    }
}
