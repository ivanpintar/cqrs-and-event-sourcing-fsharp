namespace PinetreeShop.CQRS.Infrastructure.Events
{
    public interface IHandleEvent<in TEvent> where TEvent : IEvent
    {
        IProcessManager Handle(TEvent evt);
    }
}
