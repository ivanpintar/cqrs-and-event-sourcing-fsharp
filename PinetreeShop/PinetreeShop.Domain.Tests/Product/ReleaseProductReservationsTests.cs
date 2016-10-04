using PinetreeShop.CQRS.Infrastructure.CommandsAndEvents;
using PinetreeShop.Domain.Products.Commands;
using PinetreeShop.Domain.Products.Events;
using System;
using Xunit;

namespace PinetreeShop.Domain.Tests.Product
{
    public class ReleaseProductReservationsTests : TestBase
    {
        Guid id = Guid.NewGuid();
        Guid basketId = Guid.NewGuid();

        [Fact]
        public void When_ReleaseProductResevation_NewProductHasNewQuantity()
        {

            Given(InitialEvents);
            When(new ReleaseProductReservation(id, 2));
            Then(new ProductReservationReleased(id, 2));
        }

        [Fact]
        public void When_ReleaseProductReservationLessThanReserved_ThrowQuantityChangeException()
        {
            Given(InitialEvents);
            When(new ReleaseProductReservation(id, 10));
            Then(new ProductReservationReleased(id, 2));
        }

        private IEvent[] InitialEvents
        {
            get
            {
                return new IEvent[]
                {
                    new ProductCreated(id, "Test Product", 2),
                    new ProductQuantityChanged(id, 5),
                    new ProductReserved(id, basketId, 3)
                };
            }
        }
    }
}
