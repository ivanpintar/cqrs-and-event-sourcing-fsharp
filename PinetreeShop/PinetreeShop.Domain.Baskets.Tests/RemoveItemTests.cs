using PinetreeCQRS.Infrastructure.Events;
using PinetreeShop.Domain.Baskets.Commands;
using PinetreeShop.Domain.Baskets.Events;
using PinetreeShop.Domain.Baskets.Exceptions;
using PinetreeShop.Domain.Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PinetreeShop.Domain.Baskets.Tests
{
    public class RemoveItemTests : BasketTestBase
    {
        Guid id = Guid.NewGuid();
        Guid productId = Guid.NewGuid();
        Guid causationAndCorrelationId = Guid.NewGuid();

        [Fact]
        public void When_RemoveItem_ItemRemoved()
        {
            Given(InitialEvents.ToArray());

            var command = new RemoveItemFromBasket(id, productId, 5);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            When(command);

            var expectedEvent = new BasketItemRemoved(id, productId, 5);
            expectedEvent.Metadata.CausationId = command.Metadata.CommandId;
            expectedEvent.Metadata.CorrelationId = causationAndCorrelationId;
            expectedEvent.Metadata.ProcessId = command.Metadata.ProcessId;

            Then(expectedEvent);
        }

        [Fact]
        public void When_RemoveItemBelowZero_ItemRemoved()
        {
            Given(InitialEvents.ToArray());

            var command = new RemoveItemFromBasket(id, productId, 15);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            When(command);

            var expectedEvent = new BasketItemRemoved(id, productId, 10);
            expectedEvent.Metadata.CausationId = command.Metadata.CommandId;
            expectedEvent.Metadata.CorrelationId = causationAndCorrelationId;
            expectedEvent.Metadata.ProcessId = command.Metadata.ProcessId;

            Then(expectedEvent);
        }

        [Theory]
        [InlineData("checkedOut")]
        [InlineData("cancelled")]
        public void When_RemoveItemNotPending_ThrowsInvalidStateException(string checkedOutOrCancelled)
        {
            IEvent evt = new BasketCheckedOut(id, OrderLines, new Address());
            if (checkedOutOrCancelled == "cancelled")
                evt = new BasketCancelled(id);

            InitialEvents.Add(evt);

            Given(InitialEvents.ToArray());

            WhenThrows<RemoveItemFromBasket, InvalidStateException>(new RemoveItemFromBasket(id, productId, 10));
        }

        [Fact]
        public void When_RemoveItemEmptyBasket_NothingHappens()
        {
            Given(InitialEvents.Take(1).ToArray());
            When(new RemoveItemFromBasket(id, productId, 10));
            Then(new IEvent[] { });
        }


        private List<IEvent> _initialEvents = null;
        private List<IEvent> InitialEvents
        {
            get
            {
                if (_initialEvents == null)
                {
                    _initialEvents = new List<IEvent>
                    {
                        new BasketCreated(id),
                        new BasketItemAdded(id, productId, "Test Item", 2, 10),
                    };
                }
                return _initialEvents;
            }
        }

        private List<OrderLine> OrderLines
        {
            get
            {
                return new List<OrderLine>
                {
                    new OrderLine
                    {
                        ProductName = "Test Item",
                        ProductId = productId,
                        Price = 2,
                        Quantity = 10
                    }
                };
            }
        }
    }
}

