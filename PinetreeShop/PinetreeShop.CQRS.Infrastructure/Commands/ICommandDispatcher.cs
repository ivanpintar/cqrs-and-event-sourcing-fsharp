using System;

namespace PinetreeShop.CQRS.Infrastructure.Commands
{
    public interface ICommandDispatcher
    {
        void ExecuteCommand<TAggregate>(ICommand command)
            where TAggregate : class, IAggregate, new();

        void RegisterHandler<TCommand, TAggregate>(Func<TAggregate, TCommand, TAggregate> handler)
            where TCommand : class, ICommand
            where TAggregate : class, IAggregate, new();
    }
}