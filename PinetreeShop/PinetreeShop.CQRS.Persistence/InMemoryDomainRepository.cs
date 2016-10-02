using Newtonsoft.Json;
using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Persistence.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinetreeShop.CQRS.Persistence
{
    public class InMemoryDomainRepository : DomainRepositoryBase
    {
        public Dictionary<Guid, List<string>> _eventStore = new Dictionary<Guid, List<string>>();
        private List<IEvent> _latestEvents = new List<IEvent>();
        private JsonSerializerSettings _serializationSettings;

        public InMemoryDomainRepository()
        {
            _serializationSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
        }

        public override TResult GetById<TResult>(Guid id)
        {
            if (_eventStore.ContainsKey(id))
            {
                var events = _eventStore[id];
                var deserializedEvents = events.Select(e => JsonConvert.DeserializeObject(e, _serializationSettings) as IEvent);
                return BuildAggregate<TResult>(deserializedEvents);
            }
            throw new AggregateNotFoundException($"Could not find aggregate {typeof(TResult)}:{id}");
        }

        public override IEnumerable<IEvent> Save<TAggregate>(TAggregate aggregate)
        {
            var eventsToSave = aggregate.UncommittedEvents.ToList();
            var serializedEvents = eventsToSave.Select(Serialize).ToList();
            var expectedVersion = CalculateExpectedVersion(aggregate, eventsToSave);
            if (expectedVersion < 0)
            {
                _eventStore.Add(aggregate.Id, serializedEvents);
            }
            else
            {
                var existingEvents = _eventStore[aggregate.Id];
                var currentversion = existingEvents.Count - 1;
                if (currentversion != expectedVersion)
                {
                    throw new WrongExpectedVersionException($"{aggregate.GetType()}:{aggregate.Id}: Expected version {expectedVersion} but the version is {currentversion}");
                }
                existingEvents.AddRange(serializedEvents);
            }
            _latestEvents.AddRange(eventsToSave);
            aggregate.ClearUncommittedEvents();
            return eventsToSave;
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
