using Newtonsoft.Json;
using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.CQRS.Persistence.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PinetreeShop.CQRS.Persistence
{
    public class InMemoryProcessRepository : ProcessRepositoryBase
    {
        public Dictionary<Guid, List<string>> _eventStore = new Dictionary<Guid, List<string>>();
        private List<IEvent> _latestEvents = new List<IEvent>();
        private List<ICommand> _latestCommands = new List<ICommand>();
        private JsonSerializerSettings _serializationSettings;

        public InMemoryProcessRepository()
        {
            _serializationSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
        }

        public override TResult GetProcessById<TResult>(Guid id)
        {
            if (_eventStore.ContainsKey(id))
            {
                var events = _eventStore[id].Select(e => JsonConvert.DeserializeObject(e, _serializationSettings) as IEvent);
                return BuildProcess<TResult>(events);
            }
            throw new ProcessNotFoundException($"Could not find process {typeof(TResult)}:{id}");
        }

        public override void SaveProcess<TProcess>(TProcess process)
        {
            var eventsToSave = process.UncommittedEvents.ToList();
            var serializedEvents = eventsToSave.Select(Serialize).ToList();
            var expectedVersion = CalculateExpectedVersion(process, eventsToSave);
            if (expectedVersion < 0)
            {
                _eventStore.Add(process.ProcessId, serializedEvents);
            }
            else
            {
                var existingEvents = _eventStore[process.ProcessId];
                var currentversion = existingEvents.Count - 1;
                if (currentversion != expectedVersion)
                {
                    throw new WrongExpectedVersionException($"{process.GetType()}:{process.ProcessId}: Expected version {expectedVersion} but the version is {currentversion}");
                }
                existingEvents.AddRange(serializedEvents);
            }
            _latestEvents.AddRange(eventsToSave);

            process.ClearUndispatchedCommands();
            process.ClearUncommittedEvents();
        }

        public IEnumerable<IEvent> GetLatestEvents()
        {
            return _latestEvents;
        }

        public void AddEvents(Dictionary<Guid, IEnumerable<IEvent>> eventsForProcesses)
        {
            foreach (var eventsForProcess in eventsForProcesses)
            {
                _eventStore.Add(eventsForProcess.Key, eventsForProcess.Value.Select(Serialize).ToList());
            }
        }

        private string Serialize(IEvent arg)
        {
            return JsonConvert.SerializeObject(arg, _serializationSettings);
        }
    }
}
