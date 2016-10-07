using Newtonsoft.Json;
using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.CQRS.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace PinetreeShop.Domain.Tests
{
    public class TestBase
    {
        private InMemoryAggregateRepository _aggregateRepository;
        private InMemoryProcessRepository _processRepository;
        private Dictionary<Guid, IEnumerable<IEvent>> _preConditions = new Dictionary<Guid, IEnumerable<IEvent>>();

        private DomainEntry BuildApplication()
        {
            _aggregateRepository = new InMemoryAggregateRepository();
            _aggregateRepository.AddEvents(_preConditions);

            _processRepository = new InMemoryProcessRepository();
            return new DomainEntry(_aggregateRepository, _processRepository);
        }

        protected void TearDown()
        {
            _preConditions = new Dictionary<Guid, IEnumerable<IEvent>>();
        }

        protected void Given(params IEvent[] existingEvents)
        {
            _preConditions = existingEvents
                .GroupBy(e => e.AggregateId)
                .ToDictionary(g => g.Key, g => g.AsEnumerable());
        }

        protected void When(ICommand command)
        {
            var app = BuildApplication();
            app.ExecuteCommand(command);
        }

        protected void WhenThrows<TException>(ICommand command) where TException : Exception
        {
            Assert.Throws(typeof(TException), () => When(command));
        }

        protected void Then(params IEvent[] expectedEvents)
        {
            var latestEvents = _aggregateRepository.GetLatestEvents().ToList();
            var expectedEventsList = expectedEvents != null
                ? expectedEvents.ToList()
                : new List<IEvent>();

            Assert.Equal(latestEvents.Count, expectedEventsList.Count);

            var latestAndExpected = latestEvents
                .Zip(expectedEventsList, (l, e) => new { L = l, E = e });

            foreach (var le in latestAndExpected)
            {
                Assert.True(EventsAreEqual(le.L, le.E));
            }
        }

        private bool EventsAreEqual(IEvent evt1, IEvent evt2)
        {
            // exclude metadata from comparison
            evt1.GetType().GetProperties().Single(p => p.Name == "Metadata").SetValue(evt1, null);
            evt2.GetType().GetProperties().Single(p => p.Name == "Metadata").SetValue(evt2, null);

            var json1 = JsonConvert.SerializeObject(evt1);
            var json2 = JsonConvert.SerializeObject(evt2);

            return json1 == json2;
        }
    }
}
