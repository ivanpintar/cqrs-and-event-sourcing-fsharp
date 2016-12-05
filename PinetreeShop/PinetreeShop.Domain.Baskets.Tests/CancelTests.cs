using PinetreeShop.CQRS.Infrastructure.Events;
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
    public class CancelTests : BasketTestBase
    {
        Guid id = Guid.NewGuid();
        Guid productId = Guid.NewGuid();
        Guid causationAndCorrelationId = Guid.NewGuid();
        Address shippingAddress = new Address { Country = "US", StateOrProvince = "CA", StreetAndNumber = "A2", ZipAndCity = "LA" };

        [Fact]
        public void When_Cancel_Cancelled()
        {
            Given(InitialEvents);

            var command = new CancelBasket(id);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            When(command);

            var expectedEvent = new BasketCancelled(id);
            expectedEvent.Metadata.CausationId = command.Metadata.CommandId;
            expectedEvent.Metadata.CorrelationId = causationAndCorrelationId;
            expectedEvent.Metadata.ProcessId = command.Metadata.ProcessId;

            Then(expectedEvent);
        }

        [Fact]
        public void When_CancelCheckedOut_ThrowsInvalidStateException()
        {
            var initialEvents = InitialEvents.ToList();
            initialEvents.Add(new BasketCheckedOut(id, new List<OrderLine>(), shippingAddress));
            Given(initialEvents.ToArray());
            WhenThrows<CancelBasket, InvalidStateException>(new CancelBasket(id));
        }

        [Fact]
        public void When_CancelCancelled_NothingHappens()
        {
            var initialEvents = InitialEvents.ToList();
            initialEvents.Add(new BasketCancelled(id));
            Given(initialEvents.ToArray());
            When(new CancelBasket(id));
            Then(new IEvent[] { });
        }

        private IEvent[] InitialEvents
        {
            get
            {
                return new IEvent[]
                {
                    new BasketCreated(id),
                    new BasketItemAdded(id, productId, "Test Product", 2, 10)
                };
            }
        }
    }


}
