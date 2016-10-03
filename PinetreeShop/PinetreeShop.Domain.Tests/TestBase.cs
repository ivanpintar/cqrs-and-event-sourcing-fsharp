using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.CommandsAndEvents;
using PinetreeShop.CQRS.Persistence;
using PinetreeShop.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PinetreeShop.Domain.Tests
{
    public class TestBase
    {
        private InMemoryAggregateRepository _domainRepository;
        private InMemoryWorkflowRepository _workflowRepository;
        private Dictionary<Guid, IEnumerable<IEvent>> _preConditions = new Dictionary<Guid, IEnumerable<IEvent>>();

        private DomainEntry BuildApplication()
        {
            _domainRepository = new InMemoryAggregateRepository();
            _domainRepository.AddEvents(_preConditions);

            _workflowRepository = new InMemoryWorkflowRepository();
            return new DomainEntry(_domainRepository, _workflowRepository);
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

        protected void WhenTrows<TException>(ICommand command) where TException : Exception
        {
            Assert.Throws(typeof(TException), () => When(command));
        }

        protected void Then(params IEvent[] expectedEvents)
        {
            var latestEvents = _domainRepository.GetLatestEvents().ToList();
            var expectedEventsList = expectedEvents.ToList();

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
            if (evt1.GetType() != evt2.GetType()) return false;

            var props1 = GetProps(evt1);
            var props2 = GetProps(evt2);

            if (props1.Count != props2.Count) return false;

            var allProps = props1.Zip(props2, (p1, p2) => new { P1 = p1, P2 = p2 });

            foreach (var p in allProps)
            {
                var p1 = p.P1;
                var p2 = p.P2;

                if (p1.PropertyType != p2.PropertyType) return false;
                if (p1.Name != p2.Name) return false;

                var val1 = p1.GetValue(evt1);
                var val2 = p2.GetValue(evt2);
                if (val1 != val2) return false; // false if values are different
            }

            return true;
        }

        private static List<PropertyInfo> GetProps(IEvent evt)
        {
            return evt.GetType()
                .GetProperties(BindingFlags.Instance)
                .Where(p => p.Name != "Date")
                .OrderBy(p => p.Name)
                .ToList();
        }
    }
}
