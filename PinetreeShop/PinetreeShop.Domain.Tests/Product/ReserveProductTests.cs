using PinetreeShop.Domain.Products.Commands;
using PinetreeShop.Domain.Products.Events;
using System;
using Xunit;
using PinetreeShop.CQRS.Infrastructure.Events;

namespace PinetreeShop.Domain.Tests.Product
{
    public class ReserveProductTests : AggregateTestBase
    {
        Guid id = Guid.NewGuid();
        Guid basketId = Guid.NewGuid();
        Guid causationAndCorrelationId = Guid.NewGuid();

        [Fact]
        public void When_ReserveProduct_NewProductHasNewQuantity()
        {
            Given(InitialEvents);
            
            var command = new ReserveProduct(id, basketId, 3);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            When(command);

            var expectedEvent = new ProductReserved(id, basketId, 3);
            expectedEvent.Metadata.CausationId = command.Metadata.CommandId;
            expectedEvent.Metadata.CorrelationId = causationAndCorrelationId;

            Then(expectedEvent);
        }

        [Fact]
        public void When_ReserveProductLessThanAvailable_ThrowQuantityChangeException()
        {
            Given(InitialEvents);

            var command = new ReserveProduct(id, basketId, 10);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            When(command);

            var expectedEvent = new ProductReservationFailed(id, basketId, 10, ProductReservationFailed.NotAvailable);
            expectedEvent.Metadata.CausationId = command.Metadata.CommandId;
            expectedEvent.Metadata.CorrelationId = causationAndCorrelationId;

            Then(expectedEvent);
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
