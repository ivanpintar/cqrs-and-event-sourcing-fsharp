using PinetreeShop.CQRS.Infrastructure.Repositories;
using System;
using System.Collections.Generic;

namespace PinetreeShop.CQRS.Infrastructure.Commands
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private Dictionary<Type, Func<IAggregate, ICommand, IAggregate>> _commandHandlers = new Dictionary<Type, Func<IAggregate, ICommand, IAggregate>>();
        private IAggregateRepository _aggregateRepository;

        public CommandDispatcher(IAggregateRepository aggregateRepository)
        {
            _aggregateRepository = aggregateRepository;
        }

        public void RegisterHandler<TCommand, TAggregate>(Func<TAggregate, TCommand, TAggregate> handler) 
            where TCommand : ICommand
            where TAggregate : IAggregate, new()
        {
            Func<IAggregate, ICommand, IAggregate> handler2 = (aggregate, command) =>
            {
                return handler((TAggregate)aggregate, (TCommand)command);
            };

            _commandHandlers.Add(typeof(TCommand), handler2);
        }

        public TAggregate ExecuteCommand<TAggregate>(ICommand command)
            where TAggregate : IAggregate, new()
        {
            var commandType = command.GetType();

            if (!_commandHandlers.ContainsKey(commandType))
            {
                throw new ApplicationException($"Missing handler for {commandType.Name}");
            }

            IAggregate aggregate = _aggregateRepository.GetAggregateById<TAggregate>(command.AggregateId);

            aggregate = (_commandHandlers[commandType] as Func<IAggregate, ICommand, IAggregate>)(aggregate, command);

            foreach (var evt in aggregate.UncommittedEvents)
            {
                evt.Metadata.CausationId = command.Metadata.CommandId;
                evt.Metadata.CorrelationId = command.Metadata.CorrelationId;
            }

            _aggregateRepository.SaveAggregate((TAggregate)aggregate);

            return (TAggregate)aggregate;
        }        
    }
}
