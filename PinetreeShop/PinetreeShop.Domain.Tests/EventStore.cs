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
        private List<Tuple<Type, IEvent>> _events = new List<Tuple<Type, IEvent>>();

        public List<IEvent> LatestEvents = new List<IEvent>();
        public List<ICommand> LatestCommands = new List<ICommand>();

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

        public void DispatchCommand(string queueName, ICommand command)
        {
            var cmds = new List<ICommand> { command };
            DispatchCommands(queueName, cmds);
        }

        public IEnumerable<IEvent> GetEvents(int startingPoint)
        {
            return _events.Select(t => t.Item2).Skip(startingPoint).ToList();
        }

        public IEnumerable<IEvent> GetEvents<TAggregate>(int startingPoint) where TAggregate : IAggregate
        {
            return _events
                .Where(t => t.Item1 == typeof(TAggregate))
                .Select(t => t.Item2)
                .Skip(startingPoint)
                .ToList();
        }

        public IEnumerable<IEvent> GetAggregateEvents(Guid aggregateId, int startingPoint)
        { 
            return _events
                .Select(t => t.Item2)
                .Where(e => e.AggregateId == aggregateId)
                .Skip(startingPoint)
                .ToList();
        }

        public IEnumerable<IEvent> GetProcessEvents(Guid correlatioId, int startingPoint)
        {
            return _events
                .Select(t => t.Item2)
                .Where(e => e.Metadata.CorrelationId == correlatioId)
                .Skip(startingPoint)
                .ToList();
        }

        private void AddToStream(IEvent evt, Type category, Guid id)
        {
            _events.Add(new Tuple<Type, IEvent>(category, evt));
        }
    }
}
