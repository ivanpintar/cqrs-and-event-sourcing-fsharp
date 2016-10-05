using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PinetreeShop.CQRS.Infrastructure.Events
{
    public class ProcessEventListener
    {
        private Dictionary<Type, Func<object, IProcess>> _eventListeners = new Dictionary<Type, Func<object, IProcess>>();
        private IProcessRepository _processRepository;
        private CommandDispatcher _commandDispatcher;

        public ProcessEventListener(IProcessRepository processRepository, CommandDispatcher commandDispatcher)
        {
            _processRepository = processRepository;
            _commandDispatcher = commandDispatcher;
        }

        public void RegisterHandler<TEvent>(IHandleEvent<TEvent> handler) where TEvent : class, IEvent
        {
            _eventListeners.Add(typeof(TEvent), evt => handler.Handle(evt as TEvent));
        }

        public void HandleEvent<TEvent>(TEvent evt) where TEvent : IEvent
        {
            var eventType = evt.GetType();

            if (_eventListeners.ContainsKey(eventType))
            {
                var process = _eventListeners[eventType](evt);

                var eventsToSave = process.UncommittedEvents.ToList();
                var commandsToDispatch = process.UndispatchedCommands.ToList();
                _processRepository.SaveProcess(process);
                
                foreach (var command in commandsToDispatch.ToList())
                {
                    _commandDispatcher.ExecuteCommand(command);
                }
            }
        }
    }
}
