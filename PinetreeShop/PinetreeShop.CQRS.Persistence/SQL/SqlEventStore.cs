using System;
using System.Collections.Generic;
using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Persistence.SQL.Entities;
using Newtonsoft.Json;
using System.Linq;
using System.Transactions;

namespace PinetreeShop.CQRS.Persistence.SQL
{
    public class SqlEventStore : IEventStore, ICommandQueue
    {
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
                        EventPayload = JsonConvert.SerializeObject(evt, JsonConversionSettings.SerializerSettings)
                    });
                }
                ctx.SaveChanges();
            }
        }

        public IEnumerable<ICommand> DeQueueCommands(string queueName)
        {
            using(var transaction = new TransactionScope())
            using (var ctx = new EventStoreContext())          
            {
                var commands = ctx.Commands
                    .Where(c => c.QueueName == queueName)
                    .OrderBy(c => c.Id)
                    .ToList();

                ctx.Commands.RemoveRange(commands);
                ctx.SaveChanges();

                transaction.Complete();
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
                        CommandPayload  = JsonConvert.SerializeObject(cmd, JsonConversionSettings.SerializerSettings),
                        CorrelationId = cmd.Metadata.CorrelationId,
                        QueueName = queueName                        
                    });
                }
                ctx.SaveChanges();
            }
        }
        
        public IEnumerable<IEvent> GetEvents(int lastEventNumber)
        {
            using (var ctx = new EventStoreContext())
            {
                var events = ctx.Events
                    .Where(e => e.Id > lastEventNumber)
                    .OrderBy(e => e.Id)
                    .ToList();

                return events.Select(DeserializeEvent).ToList();
            }
        }

        public IEnumerable<IEvent> GetEvents<TAggregate>(int lastEventNumber)
        {
            using (var ctx = new EventStoreContext())
            {
                var events = ctx.Events
                    .Where(e => e.Category == typeof(TAggregate).Name)
                    .Where(e => e.Id > lastEventNumber)
                    .OrderBy(e => e.Id)
                    .ToList();

                return events.Select(DeserializeEvent).ToList();
            }
        }
        public IEnumerable<IEvent> GetAggregateEvents<TAggregate>(Guid aggregateId, int lastEventNumber)
        {
            using (var ctx = new EventStoreContext())
            {
                var events = ctx.Events
                    .Where(e => e.Category == typeof(TAggregate).Name)
                    .Where(e => e.AggregateId == aggregateId)
                    .Where(e => e.Id > lastEventNumber)
                    .OrderBy(e => e.Id)
                    .ToList();

                return events.Select(DeserializeEvent).ToList();
            }
        }
        
        private ICommand DeserializeCommand(CommandEntity cmd)
        {
            return (ICommand)JsonConvert.DeserializeObject(cmd.CommandPayload, JsonConversionSettings.SerializerSettings);
        }

        private IEvent DeserializeEvent(EventEntity entity)
        {
            var evt = (IEvent)JsonConvert.DeserializeObject(entity.EventPayload, JsonConversionSettings.SerializerSettings);
            evt.Metadata.EventNumber = entity.Id;
            return evt;
        }
    };
}
