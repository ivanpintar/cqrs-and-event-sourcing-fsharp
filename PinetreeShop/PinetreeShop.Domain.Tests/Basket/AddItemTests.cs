using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.Domain.Baskets.Commands;
using PinetreeShop.Domain.Baskets.Events;
using PinetreeShop.Domain.Baskets.Exceptions;
using PinetreeShop.Domain.Types;
using System;
using System.Collections.Generic;
using Xunit;

namespace PinetreeShop.Domain.Tests.Basket
{
    public class AddItemTests : AggregateTestBase
    {
        Guid id = Guid.NewGuid();
        Guid productId = Guid.NewGuid();
        Guid causationAndCorrelationId = Guid.NewGuid();

        [Fact]
        public void When_AddItem_BasketAddItemAdded()
        {
            Given(InitialEvents.ToArray());

            var command = new AddItemToBasket(id, productId, "Test Item", 2, 10);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            When(command);

            var expectedEvent = new BasketItemAdded(id, productId, "Test Item", 2, 10);
            expectedEvent.Metadata.CausationId = command.Metadata.CommandId;
            expectedEvent.Metadata.CorrelationId = causationAndCorrelationId;

            Then(expectedEvent);
        }

        [Theory]
        [InlineData("checkedOut")]
        [InlineData("cancelled")]
        public void When_AddItemNotPending_ThrowsInvalidStateException(string checkedOutOrCancelled)
        {
            IEvent evt = new BasketCheckedOut(id, new Address());
            if (checkedOutOrCancelled == "cancelled")
                evt = new BasketCancelled(id);

            InitialEvents.Add(evt);

            Given(InitialEvents.ToArray());

            WhenThrows<InvalidStateException>(new AddItemToBasket(id, productId, "Test Product", 2, 10));
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
                        new BasketCreated(id)
                    };
                }
                return _initialEvents;
            }
        }
    }
}
