using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.Domain.Baskets.Events;
using PinetreeShop.Domain.Baskets.Commands;
using PinetreeShop.Domain.Products.Commands;
using PinetreeShop.Domain.Products.Events;
using System;
using System.Linq;
using Xunit;

namespace PinetreeShop.Domain.Tests.ShoppingProcess
{
    public class ProductReservation : ProcessManagerTestBase
    {
        Guid basketId = Guid.NewGuid();
        Guid productId = Guid.NewGuid();
        Guid causationAndCorrelationId = Guid.NewGuid();
        
        [Fact]
        public void When_ProductReserved_ConfirmAddItemToBasket()
        {
            Given(InitialEvents);

            var evt = new ProductReserved(productId, basketId, 10);
            evt.Metadata.CausationId = causationAndCorrelationId;
            evt.Metadata.CorrelationId = causationAndCorrelationId;

            When(evt);

            var cmd = new ConfirmAddItemToBasket(basketId, productId, 10);
            cmd.Metadata.CausationId = evt.Metadata.EventId;
            cmd.Metadata.CorrelationId = causationAndCorrelationId;

            Then(cmd);
        }

        [Fact]
        public void When_ProductReservationFailed_RevertAddItemToBasket()
        {
            Given(InitialEvents);

            var evt = new ProductReservationFailed(productId, basketId, 10, ProductReservationFailed.NotAvailable);
            evt.Metadata.CausationId = causationAndCorrelationId;
            evt.Metadata.CorrelationId = causationAndCorrelationId;

            When(evt);

            var cmd = new RevertAddItemToBasket(basketId, productId, 10, ProductReservationFailed.NotAvailable);
            cmd.Metadata.CausationId = evt.Metadata.EventId;
            cmd.Metadata.CorrelationId = causationAndCorrelationId;

            Then(cmd);
        }

        private IEvent[] InitialEvents
        {
            get
            {
                var created = new BasketCreated(basketId);
                created.Metadata.CausationId = causationAndCorrelationId;
                created.Metadata.CorrelationId = causationAndCorrelationId;

                var added = new BasketAddItemTried(basketId, productId, "Test Product", 2, 10);
                added.Metadata.CausationId = Guid.NewGuid();
                added.Metadata.CorrelationId = causationAndCorrelationId;

                return new IEvent[]
                {
                    created, added
                };
            }
        }
    }
}
