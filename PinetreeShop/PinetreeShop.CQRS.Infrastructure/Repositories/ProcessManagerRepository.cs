using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.CQRS.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PinetreeShop.CQRS.Infrastructure.Repositories
{
    public class ProcessManagerRepository : IProcessManagerRepository
    {
        private IEventStore _eventStore;
        private ICommandQueue _commandQueue;

        public ProcessManagerRepository(IEventStore eventStore, ICommandQueue commandQueue)
        {
            _eventStore = eventStore;
            _commandQueue = commandQueue;
        }

        public TProcessManager GetProcessManagerById<TProcessManager>(Guid id) where TProcessManager : IProcessManager, new()
        {
            var events = GetEventsForProcessManager<TProcessManager>(id);
            return BuildProcessManager<TProcessManager>(events);
        }

        public void SaveProcessManager<TProcessManager>(TProcessManager processManager) where TProcessManager : IProcessManager
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
                _commandQueue.DispatchCommands(kvp.Key.Name, kvp.Value);
            }
        }

        private List<IEvent> GetEventsForProcessManager<TProcessManager>(Guid processManagerId)
        {
            return _eventStore.GetAggregateEvents<TProcessManager>(processManagerId, 0).ToList();
        }

        private int CalculateExpectedVersion<T>(IProcessManager processManager, List<T> events)
        {
            return processManager.Version - events.Count;
        }

        private TProcessManager BuildProcessManager<TProcessManager>(IEnumerable<IEvent> events) where TProcessManager : IProcessManager, new()
        {
            var result = new TProcessManager();
            foreach (var e in events)
            {
                var evt = (e is EventProcessed) ? ((EventProcessed)e).Event : e;

                result.Transition(evt);
            }

            result.ClearUncommittedEvents();
            result.ClearUndispatchedCommands();
            return result;
        }
    }
}
