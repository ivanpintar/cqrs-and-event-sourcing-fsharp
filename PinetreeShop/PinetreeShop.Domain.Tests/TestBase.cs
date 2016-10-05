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
                Assert.True(ObjectsAreEqual(le.L, le.E));
            }
        }

        private bool ObjectsAreEqual(object evt1, object evt2)
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

                if (IsSimple(p1.PropertyType) && (val1 != val2)) return false;
                if (!ObjectsAreEqual(val1, val2)) return false;
            }

            return true;
        }

        private static List<PropertyInfo> GetProps(IEvent evt)
        {
            return GetProps(evt)
                .Where(p => p.Name != "Date" && p.Name != "EventId")
                .ToList();
        }

        private static List<PropertyInfo> GetProps(object evt)
        {
            return evt.GetType()
                .GetProperties(BindingFlags.Instance)
                .OrderBy(p => p.Name)
                .ToList();
        }

        private bool IsSimple(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // nullable type, check if the nested type is simple.
                return IsSimple(type.GetGenericArguments()[0]);
            }
            return type.IsPrimitive
              || type.IsEnum
              || type.Equals(typeof(string))
              || type.Equals(typeof(decimal));
        }
    }
}
