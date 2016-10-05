namespace PinetreeShop.CQRS.Infrastructure.Commands
{
    public interface IHandleCommand<in TCommand> where TCommand : ICommand
    {
        IAggregate Handle(TCommand command);
    }
}
