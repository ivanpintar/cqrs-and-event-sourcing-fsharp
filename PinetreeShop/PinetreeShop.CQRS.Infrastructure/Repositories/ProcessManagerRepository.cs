using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.CQRS.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PinetreeShop.CQRS.Infrastructure.Repositories
{
    public class ProcessManagerRepository : ProcessManagerRepositoryBase
    {
        private IEventStore _eventStore;

        public ProcessManagerRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public override TProcessManager GetProcessManagerById<TProcessManager>(Guid id)
        {
            var events = GetEventsForProcessManager<TProcessManager>(id);
            if (events.Any())
            {
                return BuildProcessManager<TProcessManager>(events);
            }
            return new TProcessManager();
        }

        public override void SaveProcessManager<TProcessManager>(TProcessManager processManager)
        {
            var eventsToSave = processManager.UncommittedEvents.ToList();
            var commandsToDispatch = processManager.UndispatchedCommands;
            var expectedVersion = CalculateExpectedVersion(processManager, eventsToSave);

            if (expectedVersion >= 0)
            {
                var existingEvents = GetEventsForProcessManager<TProcessManager>(processManager.ProcessId);
                var currentversion = existingEvents.Count;
                if (currentversion != expectedVersion)
                {
                    throw new WrongExpectedVersionException($"{processManager.GetType()}:{processManager.ProcessId}: Expected version {expectedVersion} but the version is {currentversion}");
                }
            }
            
            _eventStore.CommitEvents<TProcessManager>(eventsToSave);
            processManager.ClearUncommittedEvents();

            DispatchCommands(commandsToDispatch);
            processManager.ClearUndispatchedCommands();
        }

        private void DispatchCommands(Dictionary<Type, List<ICommand>> commandsToDispatch)
        {
            foreach (var kvp in commandsToDispatch)
            {
                _eventStore.DispatchCommands(kvp.Key.Name, kvp.Value);
            }
        }

        private List<IEvent> GetEventsForProcessManager<TProcessManager>(Guid processManagerId)
        {
            return _eventStore.GetProcessEvents(processManagerId, 0).ToList();
        }
    }
}
