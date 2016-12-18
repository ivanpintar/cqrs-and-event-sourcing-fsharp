using Newtonsoft.Json;
using PinetreeCQRS.Infrastructure;
using PinetreeCQRS.Infrastructure.Commands;
using PinetreeCQRS.Infrastructure.Events;
using PinetreeCQRS.Infrastructure.Repositories;
using PinetreeCQRS.Infrastructure.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PinetreeShop.Domain.Tests
{
    public abstract class AggregateTestBase<TAggregate> where TAggregate : IAggregate, new()
    {
        protected TestEventStore _eventStore = new TestEventStore();
        protected AggregateRepository _aggregateRepository;
        protected List<IEvent> _preConditions = new List<IEvent>();

        protected abstract ICommandDispatcher BuildCommandDispatcher();        

        protected void TearDown()
        {
            _preConditions.Clear();
        }

        protected void Given(params IEvent[] existingEvents)
        {
            _preConditions = existingEvents.ToList();
        }

        protected void Given(IEnumerable<IEvent> existingEvents, params IEvent[] additionalExistingEvents)
        {
            _preConditions = existingEvents.ToList();
            _preConditions.AddRange(additionalExistingEvents);
        }

        protected void When<TCommand>(TCommand command)
            where TCommand : ICommand
        {
            var dispatcher = BuildCommandDispatcher();
            dispatcher.ExecuteCommand<TAggregate>(command);
        }

        protected void WhenThrows<TCommand, TException>(TCommand command)
            where TCommand : ICommand
            where TException : Exception
        {
            Assert.Throws(typeof(TException), () => When(command));
        }

        protected void Then(params IEvent[] expectedEvents)
        {
            var latestEvents = _eventStore.LatestEvents.ToList();
            var expectedEventsList = expectedEvents != null
                ? expectedEvents.ToList()
                : new List<IEvent>();

            Assert.Equal(latestEvents.Count, expectedEventsList.Count);

            var latestAndExpected = latestEvents
                .Zip(expectedEventsList, (l, e) => new { L = l, E = e });

            foreach (var le in latestAndExpected)
            {
                Assert.True(ObjectsAreEqual(le.L, le.E));
            }
        }

        private bool ObjectsAreEqual(object evt1, object evt2)
        {
            var evtId = Guid.NewGuid();
            var now = DateTime.Now;

            // copy events to compare
            var obj1 = (dynamic)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(evt1));
            var obj2 = (dynamic)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(evt2));

            // since we're creating expected products manually, these won't ever match
            obj1.Metadata.Date = null;
            obj2.Metadata.Date = null;

            // this guid is created automatically, and we're comparing different objects, these will never match
            obj1.Metadata.EventId = null;
            obj2.Metadata.EventId = null;
            obj1.Metadata.CommandId = null;
            obj2.Metadata.CommandId = null;

            var json1 = JsonConvert.SerializeObject(obj1);
            var json2 = JsonConvert.SerializeObject(obj2);

            return json1 == json2;
        }
    }
}