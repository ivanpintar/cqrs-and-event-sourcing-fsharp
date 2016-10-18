using PinetreeShop.CQRS.Infrastructure.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PinetreeShop.CQRS.Infrastructure.Tests
{
    public class SomeEvent : EventBase
    {
        public SomeEvent(Guid aggregateId) : base(aggregateId)
        {
        }
    }

    public class EventStreamListenerTests
    {
        [Fact]
        public void When_HandlerIsRegistered_CallsHandler()
        {
            var state = 0;
            var eventStore = new TestEventStore();
            eventStore.AddPreviousEvents(new List<Tuple<Type, IEvent>> { new Tuple<Type, IEvent>(typeof(AggregateBase), new SomeEvent(Guid.NewGuid())) });

            var eventListener = new EventStreamListener(eventStore, 0);
            eventListener.RegisterEventHandler<SomeEvent>(evt => { state = 2; });

            eventListener.ReadAndHandleLatestEvents();

            Assert.True(state == 2);
        }
    }
}
