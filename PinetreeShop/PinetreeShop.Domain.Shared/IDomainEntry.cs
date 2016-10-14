using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;

namespace PinetreeShop.Domain
{
    public interface IDomainEntry
    {
        void ExecuteCommand<TCommand>(TCommand command) where TCommand : ICommand;
        void HandleEvent<TEvent>(TEvent evt) where TEvent : IEvent;
    }
}