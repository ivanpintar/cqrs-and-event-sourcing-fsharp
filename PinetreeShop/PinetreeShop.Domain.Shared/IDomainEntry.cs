using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;

namespace PinetreeShop.Domain
{
    public interface IDomainEntry
    {
        void ExecuteCommand<TCommand, TAggregate>(TCommand command)
            where TCommand : ICommand
            where TAggregate : IAggregate, new();

        void HandleEvent<TEvent, TProcessManager>(TEvent evt) 
            where TEvent : IEvent
            where TProcessManager : IProcessManager, new();
    }
}