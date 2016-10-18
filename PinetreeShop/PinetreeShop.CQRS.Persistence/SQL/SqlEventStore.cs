using System;
using System.Collections.Generic;
using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Persistence.SQL.Entities;
using Newtonsoft.Json;
using System.Linq;

namespace PinetreeShop.CQRS.Persistence.SQL
{
    public class SqlEventStore : IEventStore
    {
        private static JsonSerializerSettings _jsonSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

        public void CommitEvents<TAggregate>(IEnumerable<IEvent> events)
        {
            using (var ctx = new EventStoreContext())
            {
                foreach (var evt in events)
                {
                    ctx.Events.Add(new EventEntity
                    {
                        AggregateId = evt.AggregateId,
                        Category = typeof(TAggregate).Name,
                        CausationId = evt.Metadata.CausationId,
                        CorrelationId = evt.Metadata.CorrelationId,
                        EventId = evt.Metadata.EventId,
                        EventPayload = JsonConvert.SerializeObject(evt, _jsonSettings)
                    });
                }
                ctx.SaveChanges();
            }
        }

        public IEnumerable<ICommand> DeQueueCommands(string queueName)
        {
            using (var ctx = new EventStoreContext())
            {
                var commands = ctx.Commands
                    .Where(c => c.QueueName == queueName)
                    .OrderBy(c => c.Id)
                    .ToList();

                ctx.Commands.RemoveRange(commands);

                return commands.Select(DeserializeCommand).ToList();
            }
        }

        public void DispatchCommands(string queueName, IEnumerable<ICommand> commands)
        {
            using (var ctx = new EventStoreContext())
            {
                foreach (var cmd in commands)
                {
                    ctx.Commands.Add(new CommandEntity
                    {
                        AggregateId = cmd.AggregateId,
                        CausationId =cmd.Metadata.CausationId,
                        CommandId = cmd.Metadata.CommandId,
                        CommandPayload  = JsonConvert.SerializeObject(cmd, _jsonSettings),
                        CorrelationId = cmd.Metadata.CorrelationId,
                        QueueName = queueName                        
                    });
                }
                ctx.SaveChanges();
            }
        }

        public IEnumerable<IEvent> GetEvents(int startingPoint)
        {
            using (var ctx = new EventStoreContext())
            {
                var events = ctx.Events
                    .OrderBy(e => e.Id)
                    .Skip(startingPoint)
                    .ToList();

                return events.Select(DeserializeEvent).ToList();
            }
        }

        public IEnumerable<IEvent> GetEvents<TAggregate>(int startingPoint) where TAggregate : IAggregate
        {
            using (var ctx = new EventStoreContext())
            {
                var events = ctx.Events
                    .Where(e => e.Category == typeof(TAggregate).Name)
                    .OrderBy(e => e.Id)
                    .Skip(startingPoint)
                    .ToList();

                return events.Select(DeserializeEvent).ToList();
            }
        }

        public IEnumerable<IEvent> GetAggregateEvents<TAggregate>(Guid aggregateId, int startingPoint) where TAggregate : IAggregate
        {
            using (var ctx = new EventStoreContext())
            {
                var events = ctx.Events
                    .Where(e => e.Category == typeof(TAggregate).Name)
                    .Where(e => e.AggregateId == aggregateId)
                    .OrderBy(e => e.Id)
                    .Skip(startingPoint)
                    .ToList();

                return events.Select(DeserializeEvent).ToList();
            }
        }

        public IEnumerable<IEvent> GetProcessEvents(Guid correlationId, int startingPoint)
        {
            using (var ctx = new EventStoreContext())
            {
                var events = ctx.Events
                    .Where(e => e.CorrelationId == correlationId)
                    .OrderBy(e => e.Id)
                    .Skip(startingPoint)
                    .ToList();

                return events.Select(DeserializeEvent).ToList();
            }
        }
        
        private ICommand DeserializeCommand(CommandEntity cmd)
        {
            return JsonConvert.DeserializeObject(cmd.CommandPayload) as ICommand;
        }

        private IEvent DeserializeEvent(EventEntity evt)
        {
            return JsonConvert.DeserializeObject(evt.EventPayload) as IEvent;
        }
    }
}
