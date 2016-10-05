using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.Domain.Products.Commands;
using PinetreeShop.Domain.Products.Events;
using System;
using Xunit;

namespace PinetreeShop.Domain.Tests.Product
{
    public class CancelProductReservationsTests : TestBase
    {
        Guid id = Guid.NewGuid();
        Guid basketId = Guid.NewGuid();

        [Fact]
        public void When_CancelProductResevation_ProductReservationCancelled()
        {

            Given(InitialEvents);
            When(new CancelProductReservation(id, 2));
            Then(new ProductReservationCanceled(id, 2));
        }

        [Fact]
        public void When_CancelProductReservationLessThanReserved_ProductReservationCanceled()
        {
            Given(InitialEvents);
            When(new CancelProductReservation(id, 10));
            Then(new ProductReservationCanceled(id, 2));
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
