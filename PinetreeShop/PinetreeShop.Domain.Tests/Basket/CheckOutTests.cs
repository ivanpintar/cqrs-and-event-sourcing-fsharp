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
    public class CheckOutTests : TestBase
    {
        Guid id = Guid.NewGuid();
        Guid productId = Guid.NewGuid();
        Address shippingAddress = new Address { Country = "US", StateOrProvince = "CA", StreetAndNumber = "A2", ZipAndCity = "LA" };

        [Fact]
        public void When_CheckOut_CheckedOut()
        {
            Given(InitialEvents);
            When(new CheckOutBasket(id, shippingAddress));
            Then(new BasketCheckedOut(id, shippingAddress));
        }

        [Fact]
        public void When_CheckOutCancelled_ThrowsCheckoutException()
        {
            var initialEvents = InitialEvents.ToList();
            initialEvents.Add(new BaksetCancelled(id));
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
