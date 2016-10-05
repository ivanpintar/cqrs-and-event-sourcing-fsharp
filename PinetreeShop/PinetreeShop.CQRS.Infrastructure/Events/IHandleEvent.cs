namespace PinetreeShop.CQRS.Infrastructure.Events
{
    public interface IHandleEvent<in TEvent> where TEvent : IEvent
    {
        IProcess Handle(TEvent evt);
    }
}
