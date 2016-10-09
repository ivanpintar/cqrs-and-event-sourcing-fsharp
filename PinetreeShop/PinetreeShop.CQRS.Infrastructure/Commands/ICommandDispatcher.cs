namespace PinetreeShop.CQRS.Infrastructure.Commands
{
    public interface ICommandDispatcher
    {
        void ExecuteCommand<TCommand>(TCommand command) where TCommand : ICommand;
        void RegisterHandler<TCommand>(IHandleCommand<TCommand> handler) where TCommand : class, ICommand;
    }
}