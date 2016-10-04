using PinetreeShop.CQRS.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PinetreeShop.CQRS.Infrastructure.CommandsAndEvents
{
    public class WorkflowEventListener
    {
        private Dictionary<Type, Func<object, IWorkflow>> _eventListeners = new Dictionary<Type, Func<object, IWorkflow>>();
        private IWorkflowRepository _workflowRepository;
        private CommandDispatcher _commandDispatcher;

        public WorkflowEventListener(IWorkflowRepository workflowRepository, CommandDispatcher commandDispatcher)
        {
            _workflowRepository = workflowRepository;
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
                var workflow = _eventListeners[eventType](evt);

                var eventsToSave = workflow.UncommittedEvents.ToList();
                var commandsToDispatch = workflow.UndispatchedCommands.ToList();
                _workflowRepository.SaveWorkflow(workflow);
                
                foreach (var command in commandsToDispatch.ToList())
                {
                    _commandDispatcher.ExecuteCommand(command);
                }
            }
        }
    }
}
