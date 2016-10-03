namespace PinetreeShop.CQRS.Infrastructure.CommandsAndEvents
{
    public interface IHandleEvent<in TEvent> where TEvent : IEvent
    {
        IWorkflow Handle(TEvent evt);
    }
}
