using PinetreeShop.CQRS.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinetreeShop.CQRS.Infrastructure.Events
{
    public class EventListener
    {
        private Dictionary<Type, Func<object, IProcessManager>> _eventHandlers = new Dictionary<Type, Func<object, IProcessManager>>();
        private IProcessManagerRepository _processManagerRepository;

        public EventListener(IProcessManagerRepository processManagerRepository)
        {
            _processManagerRepository = processManagerRepository;
        }

        public void RegisterHandler<TEvent>(IHandleEvent<TEvent> handler) where TEvent : class, IEvent
        {
            _eventHandlers.Add(typeof(TEvent), evt => handler.Handle(evt as TEvent));
        }

        public void HandleEvent<TEvent>(TEvent evt) where TEvent : IEvent
        {
            var eventType = evt.GetType();

            if(!_eventHandlers.ContainsKey(eventType))
            {
                return;
            }

            var processManager = _eventHandlers[eventType](evt);
            foreach(var cmd in processManager.UndispatchedCommands)
            {
                cmd.Metadata.CausationId = evt.Metadata.EventId;
                cmd.Metadata.CorrelationId = evt.Metadata.CorrelationId;
            }
            
            _processManagerRepository.SaveProcessManager(processManager);
        }
    }
}
