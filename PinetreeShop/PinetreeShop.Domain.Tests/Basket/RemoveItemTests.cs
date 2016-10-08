using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.Domain.Baskets.Commands;
using PinetreeShop.Domain.Baskets.Events;
using PinetreeShop.Domain.Baskets.Exceptions;
using System;
using Xunit;

namespace PinetreeShop.Domain.Tests.Basket
{
    public class RemoveItemTests : TestBase
    {
        Guid id = Guid.NewGuid();
        Guid productId = Guid.NewGuid();
        Guid causationAndCorrelationId = Guid.NewGuid();

        [Fact]
        public void When_RemoveItem_ItemRemoved()
        {
            Given(InitialEvents);

            var command = new RemoveItemFromBasket(id, productId, 5);
            command.Metadata.CausationId = command.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            When(command);

            var expectedEvent = new BasketItemRemoved(id, productId, 5);
            expectedEvent.Metadata.CausationId = command.Metadata.CommandId;
            expectedEvent.Metadata.CorrelationId = causationAndCorrelationId;

            Then(expectedEvent);
        }

        [Fact]
        public void When_RemoveItemBelowZero_ItemRemoved()
        {
            Given(InitialEvents);

            var command = new RemoveItemFromBasket(id, productId, 15);
            command.Metadata.CausationId = command.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            When(command);

            var expectedEvent = new BasketItemRemoved(id, productId, 10);
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
                    new BasketCreated(id),
                    new BasketAddItemTried(id, productId, "Test Item", 2, 10),
                    new BasketAddItemConfirmed(id, productId, 10)
                };
            }
        }
    }
}
