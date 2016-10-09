using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.Domain.Baskets.Commands;
using PinetreeShop.Domain.Baskets.Events;
using System;
using System.Linq;
using Xunit;

namespace PinetreeShop.Domain.Tests.Basket
{
    public class AddItemTests :AggregateTestBase
    {
        Guid id = Guid.NewGuid();
        Guid productId = Guid.NewGuid();
        Guid causationAndCorrelationId = Guid.NewGuid();

        [Fact]
        public void When_TryAddItem_AddItemTried()
        {
            Given(InitialEvents);

            var command = new TryAddItemToBasket(id, productId, "Test Item", 2, 10);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            When(command);

            var expectedEvent = new BasketAddItemTried(id, productId, "Test Item", 2, 10);
            expectedEvent.Metadata.CausationId = command.Metadata.CommandId;
            expectedEvent.Metadata.CorrelationId = causationAndCorrelationId;

            Then(expectedEvent);
        }

        [Fact]
        public void When_ConfirmAddItem_AddItemReverted()
        {
            var events = InitialEvents.ToList();
            events.Add(new BasketAddItemTried(id, productId, "Test Item", 2, 10));
            Given(events.ToArray());

            var command = new ConfirmAddItemToBasket(id, productId, 10);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            When(command);

            var expectedEvent = new BasketAddItemConfirmed(id, productId, 10);
            expectedEvent.Metadata.CausationId = command.Metadata.CommandId;
            expectedEvent.Metadata.CorrelationId = causationAndCorrelationId;

            Then(expectedEvent);
        }

        [Fact]
        public void When_RevertAddItem_AddItemReverted()
        {
            var events = InitialEvents.ToList();
            events.Add(new BasketAddItemTried(id, productId, "Test Item", 2, 10));
            Given(events.ToArray());
          
            var command = new RevertAddItemToBasket(id, productId, 10, "reason");
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            When(command);

            var expectedEvent = new BasketAddItemReverted(id, productId, 10, "reason");
            expectedEvent.Metadata.CausationId = command.Metadata.CommandId;
            expectedEvent.Metadata.CorrelationId = causationAndCorrelationId;

            Then(expectedEvent);
        }

        private IEvent[] InitialEvents
        {
            get
            {
                return new IEvent[]
                {
                    new BasketCreated(id)
                };
            }
        }
    }
}
