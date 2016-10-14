using System;

namespace PinetreeShop.CQRS.Infrastructure.Commands
{
    public interface ICommandDispatcher
    {
        void ExecuteCommand<TCommand, TAggregate>(TCommand command) 
            where TCommand : ICommand 
            where TAggregate : IAggregate, new();

        void RegisterHandler<TCommand, TAggregate>(Func<TAggregate, TCommand, TAggregate> handler) 
            where TCommand : class, ICommand
            where TAggregate : IAggregate;
    }
}