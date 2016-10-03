using PinetreeShop.Domain.Products.Commands;
using PinetreeShop.Domain.Products.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using PinetreeShop.CQRS.Infrastructure.CommandsAndEvents;

namespace PinetreeShop.Domain.Tests.Product
{
    public class ReserveProductTests : TestBase
    {
        [Fact]
        public void When_ReserveProduct_NewProductHasNewQuantity()
        {
            Guid id = Guid.NewGuid();
            Guid basketId = Guid.NewGuid();

            Given(CreateInitialEvents(id));
            When(new ReserveProduct(id, basketId, 3));
            Then(new ProductReserved(id, basketId, 3));
        }

        [Fact]
        public void When_ReserveProductLessThanAvailable_ThrowQuantityChangeException()
        {
            Guid id = Guid.NewGuid();
            Guid basketId = Guid.NewGuid();

            Given(CreateInitialEvents(id));
            When(new ReserveProduct(id, basketId, 10));
            Then(new ProductReservationFailed(id, basketId, 10, ProductReservationFailed.NotAvailable));
        }

        private IEvent[] CreateInitialEvents(Guid id)
        {
            return new IEvent[]
            {
                new ProductCreated(id, "new product", 2),
                new ProductQuantityChanged(id, 5),
                new ProductReserved(id, Guid.NewGuid(), 2)
            };
        }
    }
}
