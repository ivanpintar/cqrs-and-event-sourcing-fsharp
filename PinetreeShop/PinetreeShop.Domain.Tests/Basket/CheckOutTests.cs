using System;
using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.Domain.Baskets.Events;
using PinetreeShop.Domain.Types;
using Xunit;
using PinetreeShop.Domain.Baskets.Commands;
using System.Linq;
using PinetreeShop.Domain.Baskets.Exceptions;

namespace PinetreeShop.Domain.Tests.Basket
{
    public class CheckOutTests : AggregateTestBase
    {
        Guid id = Guid.NewGuid();
        Guid productId = Guid.NewGuid();
        Guid causationAndCorrelationId = Guid.NewGuid();
        Address shippingAddress = new Address { Country = "US", StateOrProvince = "CA", StreetAndNumber = "A2", ZipAndCity = "LA" };

        [Fact]
        public void When_CheckOut_CheckedOut()
        {
            Given(InitialEvents);

            var command = new CheckOutBasket(id, shippingAddress);
            command.Metadata.CausationId = command.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            When(command);

            var expectedEvent = new BasketCheckedOut(id, shippingAddress);
            expectedEvent.Metadata.CausationId = command.Metadata.CommandId;
            expectedEvent.Metadata.CorrelationId = causationAndCorrelationId;

            Then(expectedEvent);
        }

        [Fact]
        public void When_CheckOutCancelled_ThrowsCheckoutException()
        {
            var initialEvents = InitialEvents.ToList();
            initialEvents.Add(new BasketCancelled(id));
            Given(initialEvents.ToArray());
            WhenThrows<CheckoutException>(new CheckOutBasket(id, shippingAddress));
        }
        
        private IEvent[] InitialEvents
        {
            get
            {
                return new IEvent[]
                {
                    new BasketCreated(id),
                    new BasketAddItemTried(id, productId, "Test Item", 2, 10)
                };
            }
        }
    }
}
