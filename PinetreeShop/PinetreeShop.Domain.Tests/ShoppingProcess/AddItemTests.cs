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
    public class AddItemTests : ProcessManagerTestBase
    {
        Guid basketId = Guid.NewGuid();
        Guid productId = Guid.NewGuid();
        Guid causationAndCorrelationId = Guid.NewGuid();

        [Fact]
        public void When_BasketAddItemTried_ReserveProduct()
        {
            Given(InitialEvents);

            var evt = new BasketAddItemTried(basketId, productId, "Test product", 2, 10);
            evt.Metadata.CausationId = causationAndCorrelationId;
            evt.Metadata.CorrelationId = causationAndCorrelationId;

            When(evt);

            var cmd = new ReserveProduct(productId, basketId, 10);
            cmd.Metadata.CausationId = evt.Metadata.EventId;
            cmd.Metadata.CorrelationId = causationAndCorrelationId;

            Then(cmd);
        }

        [Fact]
        public void When_ProductReserved_ConfirmAddItemToBasket()
        {
            var events = InitialEvents.ToList();
            events.Add(new BasketAddItemTried(basketId, productId, "Test Product", 2, 10));
            Given(events.ToArray());

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
            var events = InitialEvents.ToList();
            events.Add(new BasketAddItemTried(basketId, productId, "Test Product", 2, 10));
            Given(events.ToArray());

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

                return new IEvent[]
                {
                    created
                };
            }
        }
    }
}
