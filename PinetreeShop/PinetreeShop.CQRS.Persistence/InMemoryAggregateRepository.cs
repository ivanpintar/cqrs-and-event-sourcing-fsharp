using Newtonsoft.Json;
using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.CQRS.Persistence.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PinetreeShop.CQRS.Persistence
{
    public class InMemoryAggregateRepository : AggregateRepositoryBase
    {
        public Dictionary<Guid, List<string>> _eventStore = new Dictionary<Guid, List<string>>();
        private List<IEvent> _latestEvents = new List<IEvent>();
        private List<ICommand> _latestCommands = new List<ICommand>();
        private JsonSerializerSettings _serializationSettings;

        public InMemoryAggregateRepository()
        {
            _serializationSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
        }

        public override TResult GetAggregateById<TResult>(Guid id)
        {
            if (_eventStore.ContainsKey(id))
            {
                var events = _eventStore[id].Select(e => JsonConvert.DeserializeObject(e, _serializationSettings) as IEvent);
                return BuildAggregate<TResult>(events);
            }
            throw new AggregateNotFoundException($"Could not find aggregate {typeof(TResult)}:{id}");
        }

        public override void SaveAggregate<TAggregate>(TAggregate aggregate)
        {
            var eventsToSave = aggregate.UncommittedEvents.ToList();
            var serializedEvents = eventsToSave.Select(Serialize).ToList();
            var expectedVersion = CalculateExpectedVersion(aggregate, eventsToSave);
            if (expectedVersion < 0)
            {
                _eventStore.Add(aggregate.AggregateId, serializedEvents);
            }
            else
            {
                var existingEvents = _eventStore[aggregate.AggregateId];
                var currentversion = existingEvents.Count - 1;
                if (currentversion != expectedVersion)
                {
                    throw new WrongExpectedVersionException($"{aggregate.GetType()}:{aggregate.AggregateId}: Expected version {expectedVersion} but the version is {currentversion}");
                }
                existingEvents.AddRange(serializedEvents);
            }
            _latestEvents.AddRange(eventsToSave);
            aggregate.ClearUncommittedEvents();
        }
       
        public IEnumerable<IEvent> GetLatestEvents()
        {
            return _latestEvents;
        }

        public void AddEvents(Dictionary<Guid, IEnumerable<IEvent>> eventsForAggregates)
        {
            foreach (var eventsForAggregate in eventsForAggregates)
            {
                _eventStore.Add(eventsForAggregate.Key, eventsForAggregate.Value.Select(Serialize).ToList());
            }
        }

        private string Serialize(IEvent arg)
        {
            return JsonConvert.SerializeObject(arg, _serializationSettings);
        }
    }
}
