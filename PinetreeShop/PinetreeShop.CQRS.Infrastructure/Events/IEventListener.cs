namespace PinetreeShop.CQRS.Infrastructure.Events
{
    public interface IEventListener
    {
        void HandleEvent<TEvent>(TEvent evt) where TEvent : IEvent;
        void RegisterHandler<TEvent>(IHandleEvent<TEvent> handler) where TEvent : class, IEvent;
    }
}