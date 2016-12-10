using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;
using System.Collections.Generic;
using System.Linq;
using System;

namespace PinetreeShop.CQRS.Infrastructure.Tests
{
    public class TestEventStore : IEventStore
    {
        private List<Tuple<Type, IEvent>> _events = new List<Tuple<Type, IEvent>>();

        public List<IEvent> LatestEvents = new List<IEvent>();

        public void AddPreviousEvents(List<Tuple<Type, IEvent>> preConditions)
        {
            _events.AddRange(preConditions);
        }

        public void AddPreviousEvents<TAggregate>(List<IEvent> preConditions)
        {
            var tuples = preConditions.Select(e => new Tuple<Type, IEvent>(typeof(TAggregate), e)).ToList();
            AddPreviousEvents(tuples);
        }

        public void CommitEvents<TAggregate>(IEnumerable<IEvent> events)
        {
            foreach (var evt in events)
            {
                AddToStream(evt, typeof(TAggregate), evt.AggregateId);
                LatestEvents.Add(evt);
            }
        }
        
        public IEnumerable<IEvent> GetEvents(int startingPoint)
        {
            return _events.Select(t => t.Item2).Skip(startingPoint).ToList();
        }

        public IEnumerable<IEvent> GetEvents<TAggregate>(int startingPoint)
        {
            return _events
                .Where(t => t.Item1 == typeof(TAggregate))
                .Select(t => t.Item2)
                .Skip(startingPoint)
                .ToList();
        }
        
        public IEnumerable<IEvent> GetAggregateEvents<TAggregate>(Guid aggregateId, int startingPoint)
        {
            return _events
                .Where(t => t.Item1 == typeof(TAggregate))
                .Select(t => t.Item2)
                .Where(e => e.AggregateId == aggregateId)
                .Skip(startingPoint)
                .ToList();
        }

        private void AddToStream(IEvent evt, Type category, Guid id)
        {
            _events.Add(new Tuple<Type, IEvent>(category, evt));
        }
    }
}
