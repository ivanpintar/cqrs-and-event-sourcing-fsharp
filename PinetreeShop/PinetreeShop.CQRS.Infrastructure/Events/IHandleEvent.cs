namespace PinetreeShop.CQRS.Infrastructure.Events
{
    public interface IHandleEvent<in TEvent> where TEvent : IEvent
    {
        void Handle(TEvent evt);
    }
}
