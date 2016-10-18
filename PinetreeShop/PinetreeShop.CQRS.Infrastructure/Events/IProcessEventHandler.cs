using System;

namespace PinetreeShop.CQRS.Infrastructure.Events
{
    public interface IProcessEventHandler
    {
        void HandleEvent<TEvent, TProcessManager>(TEvent command)
            where TEvent : IEvent
            where TProcessManager : IProcessManager, new();

        void RegisterHandler<TEvent, TProcessManager>(Func<TProcessManager, TEvent, TProcessManager> handler)
            where TEvent : class, IEvent
            where TProcessManager : IProcessManager;
    }
}