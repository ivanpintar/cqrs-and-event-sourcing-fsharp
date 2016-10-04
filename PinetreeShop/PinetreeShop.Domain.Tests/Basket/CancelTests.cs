using PinetreeShop.CQRS.Infrastructure.CommandsAndEvents;
using PinetreeShop.Domain.Baskets.Commands;
using PinetreeShop.Domain.Baskets.Events;
using PinetreeShop.Domain.Baskets.Exceptions;
using PinetreeShop.Domain.Types;
using System;
using System.Linq;
using Xunit;

namespace PinetreeShop.Domain.Tests.Basket
{
    public class CancelTests : TestBase
    {
        Guid id = Guid.NewGuid();
        Guid productId = Guid.NewGuid();
        Address shippingAddress = new Address { Country = "US", StateOrProvince = "CA", StreetAndNumber = "A2", ZipAndCity = "LA" };

        [Fact]
        public void When_Cancel_Cancelled()
        {
            Given(InitialEvents);
            When(new Cancel(id));
            Then(new Cancelled(id));
        }

        [Fact]
        public void When_CancelCheckedOut_ThrowsCheckoutException()
        {
            var initialEvents = InitialEvents.ToList();
            initialEvents.Add(new CheckedOut(id, shippingAddress));
            Given(initialEvents.ToArray());
            WhenThrows<CancellationException>(new Cancel(id));
        }

        [Fact]
        public void When_CancelCancelled_NothingHappens()
        {
            var initialEvents = InitialEvents.ToList();
            initialEvents.Add(new Cancelled(id));
            Given(initialEvents.ToArray());
            When(new Cancel(id));
            Then();
        }

        private IEvent[] InitialEvents
        {
            get
            {
                return new IEvent[]
                {
                    new BasketCreated(id),
                    new ProductAdded(id, productId, "Test Product", 2, 10)
                };
            }
        }
    }


}
