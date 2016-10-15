using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;
using System.Collections.Generic;
using System.Linq;
using System;

namespace PinetreeShop.Domain.Tests
{
    public class TestEventStore : IEventStore
    {
        private Dictionary<string, List<ICommand>> _commandQueues = new Dictionary<string, List<ICommand>>();
        private Dictionary<string, Dictionary<Guid, List<IEvent>>> _eventStreams = new Dictionary<string, Dictionary<Guid, List<IEvent>>>();

        public void AddPreviousEvents(List<Tuple<Type, IEvent>> preConditions)
        {
            foreach (var t in preConditions)
            {
                AddToStream(t.Item2, t.Item1.Name, t.Item2.AggregateId);
            }
        }

        public void AddPreviousEvents<TAggregate>(List<IEvent> preConditions)
        {
            var tuples = preConditions.Select(e => new Tuple<Type, IEvent>(typeof(TAggregate), e)).ToList();
            AddPreviousEvents(tuples);
        }


        public List<IEvent> LatestEvents = new List<IEvent>();
        public List<ICommand> LatestCommands = new List<ICommand>();

        public void CommitEvents<TAggregate>(IEnumerable<IEvent> events)
        {
            foreach (var evt in events)
            {
                AddToStream(evt, typeof(TAggregate).Name, evt.AggregateId);
                LatestEvents.Add(evt);
            }
        }

        public IEnumerable<ICommand> DeQueueCommands(string queueName)
        {
            var commands = _commandQueues[queueName].ToList();
            _commandQueues[queueName].Clear();
            return commands;
        }

        public void DispatchCommands(string queueName, IEnumerable<ICommand> commands)
        {
            foreach (var cmd in commands)
            {
                if (!_commandQueues.ContainsKey(queueName))
                    _commandQueues[queueName] = new List<ICommand>();

                _commandQueues[queueName].Add(cmd);
                LatestCommands.Add(cmd);
            }
        }

        public IEnumerable<IEvent> GetEvents(string category, int startingPoint)
        {
            if (!_eventStreams.ContainsKey(category)) return Enumerable.Empty<IEvent>();

            return _eventStreams[category].SelectMany(c => c.Value).Skip(startingPoint);
        }

        public IEnumerable<IEvent> GetEvents(string category, Guid id, int startingPoint)
        {
            if (!_eventStreams.ContainsKey(category) || !_eventStreams[category].ContainsKey(id))
                    return Enumerable.Empty<IEvent>();

            return _eventStreams[category][id].Skip(startingPoint);
        }
        
        private void AddToStream(IEvent evt, string category, Guid id)
        {
            if (!_eventStreams.ContainsKey(category))
                _eventStreams[category] = new Dictionary<Guid, List<IEvent>>();

            if (!_eventStreams[category].ContainsKey(id))
                _eventStreams[category][id] = new List<IEvent>();

            _eventStreams[category][id].Add(evt);


            if (category != "OrderProcessManager")
            {
                // add to process manager projected stream
                AddToStream(evt, "OrderProcessManager", evt.Metadata.CorrelationId);
            }
        }
    }
}
