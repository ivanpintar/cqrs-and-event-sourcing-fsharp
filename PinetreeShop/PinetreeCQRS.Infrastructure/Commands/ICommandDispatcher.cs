using System;

namespace PinetreeCQRS.Infrastructure.Commands
{
    public interface ICommandDispatcher
    {
        TAggregate ExecuteCommand<TAggregate>(ICommand command)
            where TAggregate : IAggregate, new();

        void RegisterHandler<TCommand, TAggregate>(Func<TAggregate, TCommand, TAggregate> handler)
            where TCommand : ICommand
            where TAggregate : IAggregate, new();
    }
}