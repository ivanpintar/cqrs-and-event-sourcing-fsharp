using PinetreeShop.CQRS.Infrastructure.CommandsAndEvents;
using PinetreeShop.Domain.Products.Commands;
using PinetreeShop.Domain.Products.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PinetreeShop.Domain.Tests.Product
{
    public class ReleaseProductReservationsTests :TestBase
    {
        [Fact]
        public void When_ReleaseProductResevation_NewProductHasNewQuantity()
        {
            Guid id = Guid.NewGuid();
            Guid basketId = Guid.NewGuid();

            Given(CreateInitialEvents(id));
            When(new ReleaseProductReservation(id, 2));
            Then(new ProductReservationReleased(id, 2));
        }

        [Fact]
        public void When_ReleaseProductReservationLessThanReserved_ThrowQuantityChangeException()
        {
            Guid id = Guid.NewGuid();
            Guid basketId = Guid.NewGuid();

            Given(CreateInitialEvents(id));
            When(new ReleaseProductReservation(id, 10));
            Then(new ProductReservationReleased(id, 2));
        }

        private IEvent[] CreateInitialEvents(Guid id)
        {
            return new IEvent[]
            {
                new ProductCreated(id, "new product", 2),
                new ProductQuantityChanged(id, 5),
                new ProductReserved(id, Guid.NewGuid(), 3)
            };
        }
    }
}
