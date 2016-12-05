using PinetreeShop.CQRS.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinetreeShop.CQRS.Infrastructure.Events
{
    public class ProcessEventHandler : IProcessEventHandler
    {
        private Dictionary<Type, object> _eventHandlers = new Dictionary<Type, object>();
        private IProcessManagerRepository _processManagerRepository;

        public ProcessEventHandler(IProcessManagerRepository processManagerRepository)
        {
            _processManagerRepository = processManagerRepository;
        }

        public void RegisterHandler<TEvent, TProcessManager>(Func<TProcessManager, TEvent, TProcessManager> handler)
            where TEvent : class, IEvent
            where TProcessManager : IProcessManager

        {
            _eventHandlers.Add(typeof(TEvent), handler);
        }

        public void HandleEvent<TEvent, TProcessManager>(TEvent evt)
            where TEvent : IEvent
            where TProcessManager : IProcessManager, new()
        {
            var eventType = evt.GetType();

            if (!_eventHandlers.ContainsKey(eventType))
            {
                return;
            }

            var processManager = _processManagerRepository.GetProcessManagerById<TProcessManager>(evt.Metadata.ProcessId);

            processManager = (_eventHandlers[eventType] as Func<TProcessManager, TEvent, TProcessManager>)(processManager, evt);

            foreach (var cmd in processManager.UndispatchedCommands.SelectMany(x => x.Value))
            {
                cmd.Metadata.CausationId = evt.Metadata.EventId;
                cmd.Metadata.CorrelationId = evt.Metadata.CorrelationId;
                cmd.Metadata.ProcessId = evt.Metadata.ProcessId;
            }

            foreach (var e in processManager.UncommittedEvents)
            {
                e.Metadata.CausationId = evt.Metadata.EventId;
                e.Metadata.CorrelationId = evt.Metadata.CorrelationId;
                e.Metadata.ProcessId = evt.Metadata.ProcessId;
            }

            _processManagerRepository.SaveProcessManager(processManager);
        }
    }
}
