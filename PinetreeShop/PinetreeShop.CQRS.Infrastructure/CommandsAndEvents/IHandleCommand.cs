namespace PinetreeShop.CQRS.Infrastructure.CommandsAndEvents
{
    public interface IHandleCommand<in TCommand> where TCommand : ICommand
    {
        IAggregate Handle(TCommand command);
    }
}
