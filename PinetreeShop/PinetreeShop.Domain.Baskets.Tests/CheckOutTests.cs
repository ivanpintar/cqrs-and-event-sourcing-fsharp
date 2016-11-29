using System;
using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.Domain.Baskets.Events;
using Xunit;
using PinetreeShop.Domain.Baskets.Commands;
using System.Linq;
using PinetreeShop.Domain.Baskets.Exceptions;
using System.Collections.Generic;
using PinetreeShop.Domain.Shared.Types;

namespace PinetreeShop.Domain.Baskets.Tests
{
    public class CheckOutTests : BasketTestBase
    {
        Guid id = Guid.NewGuid();
        Guid productId = Guid.NewGuid();
        Guid causationAndCorrelationId = Guid.NewGuid();
        Address shippingAddress = new Address { Country = "US", StateOrProvince = "CA", StreetAndNumber = "A2", ZipAndCity = "LA" };

        [Fact]
        public void When_CheckOut_CheckedOut()
        {
            Given(InitialEvents.ToArray());

            var command = new CheckOutBasket(id, shippingAddress);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            When(command);

            var expectedEvent = new BasketCheckedOut(id, OrderLines, shippingAddress);
            expectedEvent.Metadata.CausationId = command.Metadata.CommandId;
            expectedEvent.Metadata.CorrelationId = causationAndCorrelationId;

            Then(expectedEvent);
        }

        [Fact]
        public void When_CheckOutEmpty_NothingHappens()
        {
            Given(InitialEvents.Take(1).ToArray());
            When(new CheckOutBasket(id, shippingAddress));
            Then(new IEvent[] { });
        }

        [Fact]
        public void When_CheckOutCancelled_ThrowsInvalidStateException()
        {
            InitialEvents.Add(new BasketCancelled(id));
            Given(InitialEvents.ToArray());
            WhenThrows<CheckOutBasket, InvalidStateException>(new CheckOutBasket(id, shippingAddress));
        }

        [Fact]
        public void When_CheckOutCheckedOut_NothingHappens()
        {
            var initialEvents = InitialEvents.ToList();
            initialEvents.Add(new BasketCheckedOut(id, OrderLines, shippingAddress));
            Given(initialEvents.ToArray());
            When(new CheckOutBasket(id, shippingAddress));
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
                        new BasketItemAdded(id, productId, "Test Item", 2, 10)
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
