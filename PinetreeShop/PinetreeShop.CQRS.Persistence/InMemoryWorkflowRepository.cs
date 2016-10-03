using Newtonsoft.Json;
using PinetreeShop.CQRS.Infrastructure.CommandsAndEvents;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.CQRS.Persistence.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PinetreeShop.CQRS.Persistence
{
    public class InMemoryWorkflowRepository : WorkflowRepositoryBase
    {
        public Dictionary<Guid, List<string>> _eventStore = new Dictionary<Guid, List<string>>();
        private List<IEvent> _latestEvents = new List<IEvent>();
        private List<ICommand> _latestCommands = new List<ICommand>();
        private JsonSerializerSettings _serializationSettings;

        public InMemoryWorkflowRepository()
        {
            _serializationSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
        }

        public override TResult GetWorkflowById<TResult>(Guid id)
        {
            if (_eventStore.ContainsKey(id))
            {
                var events = _eventStore[id].Select(e => JsonConvert.DeserializeObject(e, _serializationSettings) as IEvent);
                return BuildWorkflow<TResult>(events);
            }
            throw new WorkflowNotFoundException($"Could not find workflow {typeof(TResult)}:{id}");
        }

        public override void SaveWorkflow<TWorkflow>(TWorkflow workflow)
        {
            var eventsToSave = workflow.UncommittedEvents.ToList();
            var serializedEvents = eventsToSave.Select(Serialize).ToList();
            var expectedVersion = CalculateExpectedVersion(workflow, eventsToSave);
            if (expectedVersion < 0)
            {
                _eventStore.Add(workflow.Id, serializedEvents);
            }
            else
            {
                var existingEvents = _eventStore[workflow.Id];
                var currentversion = existingEvents.Count - 1;
                if (currentversion != expectedVersion)
                {
                    throw new WrongExpectedVersionException($"{workflow.GetType()}:{workflow.Id}: Expected version {expectedVersion} but the version is {currentversion}");
                }
                existingEvents.AddRange(serializedEvents);
            }
            _latestEvents.AddRange(eventsToSave);

            workflow.ClearUndispatchedCommands();
            workflow.ClearUncommittedEvents();
        }

        public IEnumerable<IEvent> GetLatestEvents()
        {
            return _latestEvents;
        }

        public void AddEvents(Dictionary<Guid, IEnumerable<IEvent>> eventsForWorflows)
        {
            foreach (var eventsForWorflow in eventsForWorflows)
            {
                _eventStore.Add(eventsForWorflow.Key, eventsForWorflow.Value.Select(Serialize).ToList());
            }
        }

        private string Serialize(IEvent arg)
        {
            return JsonConvert.SerializeObject(arg, _serializationSettings);
        }
    }
}
