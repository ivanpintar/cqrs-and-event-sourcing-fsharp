using Newtonsoft.Json;
using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.CQRS.Persistence;
using PinetreeShop.CQRS.Persistence.InMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PinetreeShop.Domain.Tests
{
    public class TestBase
    {
        protected TestEventStore _eventStore = new TestEventStore();
        protected AggregateRepository _aggregateRepository;
        protected List<IEvent> _preConditions = new List<IEvent>();
        protected ProcessManagerRepository _processManagerRepository;

        protected DomainEntry BuildApplication()
        {
            _eventStore.AddPreviousEvents(_preConditions);
            _aggregateRepository = new AggregateRepository(_eventStore);
            var commandDispatcher = new CommandDispatcher(_aggregateRepository);
            _processManagerRepository = new ProcessManagerRepository(_eventStore);
            var eventListener = new EventListener(_processManagerRepository);

            return new DomainEntry(commandDispatcher, eventListener, _aggregateRepository, _processManagerRepository);
        }

        protected void TearDown()
        {
            _preConditions.Clear();
        }

        protected void Given(params IEvent[] existingEvents)
        {
            _preConditions = existingEvents.ToList();
        }

        protected void When(ICommand command)
        {
            var app = BuildApplication();
            app.ExecuteCommand(command);
        }

        protected void When(IEvent evt)
        {
            var app = BuildApplication();
            app.HandleEvent(evt);
        }

        protected void WhenThrows<TException>(ICommand command) where TException : Exception
        {
            Assert.Throws(typeof(TException), () => When(command));
        }

        protected void Then(params IEvent[] expectedEvents)
        {
            var latestEvents = _eventStore.GetLatestEvents().ToList();
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

        protected void Then(params ICommand[] expectedCommands)
        {
            var latestCommands = _eventStore.GetLatestCommands();
            var expectedCommandsList = expectedCommands != null
                ? expectedCommands.ToList()
                : new List<ICommand>();

            Assert.Equal(latestCommands.Count(), expectedCommandsList.Count);

            var latestAndExpected = latestCommands.Zip(expectedCommandsList, (l, e) => new { L = l, E = e });

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