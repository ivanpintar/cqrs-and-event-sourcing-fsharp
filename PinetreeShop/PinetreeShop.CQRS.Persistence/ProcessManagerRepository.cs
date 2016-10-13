using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.CQRS.Persistence.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PinetreeShop.CQRS.Persistence
{
    public class ProcessManagerRepository : ProcessManagerRepositoryBase
    {
        private IEventStore _eventStore;

        public ProcessManagerRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }
        
        public override TResult GetProcessManagerById<TResult>(Guid id)
        {
            var events = GetEventsForProcessManager(id);
            if (events.Any())
            {
                return BuildProcessManager<TResult>(events);
            }
            throw new ProcessManagerNotFoundException($"Could not find process manager {typeof(TResult)}:{id}");
        }

        public override void SaveProcessManager<TProcessManager>(TProcessManager processManager)
        {
            var handledEvents = processManager.UncommittedEvents.ToList();
            var commandsToDispatch = processManager.UndispatchedCommands.ToList();
            var expectedVersion = CalculateExpectedVersion(processManager, handledEvents);

            if(expectedVersion >= 0)
            {
                var existingEvents = GetEventsForProcessManager(processManager.ProcessId);
                var currentversion = existingEvents.Count;
                if (currentversion != expectedVersion)
                {
                    throw new WrongExpectedVersionException($"{processManager.GetType()}:{processManager.ProcessId}: Expected version {expectedVersion} but the version is {currentversion}");
                }                
            }

            DispatchCommands(commandsToDispatch);

            processManager.ClearUncommittedEvents();
            processManager.ClearUndispatchedCommands();
        }

        private void DispatchCommands(List<ICommand> commandsToDispatch)
        {
            _eventStore.DispatchCommands(commandsToDispatch);
        }

        private List<IEvent> GetEventsForProcessManager(Guid processManagerId)
        {
            return _eventStore.Events.Where(e => e.Metadata.CorrelationId == processManagerId).ToList();
        }
    }
}
