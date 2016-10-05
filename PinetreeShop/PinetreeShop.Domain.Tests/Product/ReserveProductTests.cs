using PinetreeShop.Domain.Products.Commands;
using PinetreeShop.Domain.Products.Events;
using System;
using Xunit;
using PinetreeShop.CQRS.Infrastructure.Events;

namespace PinetreeShop.Domain.Tests.Product
{
    public class ReserveProductTests : TestBase
    {
        Guid id = Guid.NewGuid();
        Guid basketId = Guid.NewGuid();

        [Fact]
        public void When_ReserveProduct_NewProductHasNewQuantity()
        {
            Given(InitialEvents);
            When(new ReserveProduct(id, basketId, 3));
            Then(new ProductReserved(id, basketId, 3));
        }

        [Fact]
        public void When_ReserveProductLessThanAvailable_ThrowQuantityChangeException()
        {
            Given(InitialEvents);
            When(new ReserveProduct(id, basketId, 10));
            Then(new ProductReservationFailed(id, basketId, 10, ProductReservationFailed.NotAvailable));
        }

        private IEvent[] InitialEvents
        {
            get
            {
                return new IEvent[]
                {
                    new ProductCreated(id, "Test Product", 2),
                    new ProductQuantityChanged(id, 5),
                    new ProductReserved(id, basketId, 2)
                };
            }
        }
    }
}
