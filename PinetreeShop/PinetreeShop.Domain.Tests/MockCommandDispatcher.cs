using PinetreeShop.CQRS.Infrastructure.Commands;

namespace PinetreeShop.Domain.Tests
{
    public class MockCommandDispatcher : ICommandDispatcher
    {
        public void ExecuteCommand<TCommand>(TCommand command) where TCommand : ICommand
        {
            
        }

        void ICommandDispatcher.RegisterHandler<TCommand>(IHandleCommand<TCommand> handler)
        {
            
        }
    }
}
