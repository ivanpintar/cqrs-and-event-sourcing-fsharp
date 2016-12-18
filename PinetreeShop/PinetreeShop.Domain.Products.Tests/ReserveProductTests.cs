using PinetreeShop.Domain.Products.Commands;
using PinetreeShop.Domain.Products.Events;
using System;
using Xunit;
using PinetreeCQRS.Infrastructure.Events;

namespace PinetreeShop.Domain.Products.Tests
{
    public class ReserveProductTests : ProductTestBase
    {
        Guid id = Guid.NewGuid();
        Guid basketId = Guid.NewGuid();
        Guid causationAndCorrelationId = Guid.NewGuid();

        [Fact]
        public void When_ReserveProduct_NewProductHasNewQuantity()
        {
            Given(InitialEvents);
            
            var command = new ReserveProduct(id, 3);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            When(command);

            var expectedEvent = new ProductReserved(id, 3);
            expectedEvent.Metadata.CausationId = command.Metadata.CommandId;
            expectedEvent.Metadata.CorrelationId = causationAndCorrelationId;
            expectedEvent.Metadata.ProcessId = command.Metadata.ProcessId;

            Then(expectedEvent);
        }

        [Fact]
        public void When_ReserveProductLessThanAvailable_ThrowQuantityChangeException()
        {
            Given(InitialEvents);

            var command = new ReserveProduct(id, 10);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            When(command);

            var expectedEvent = new ProductReservationFailed(id, 10, ProductReservationFailed.NotAvailable);
            expectedEvent.Metadata.CausationId = command.Metadata.CommandId;
            expectedEvent.Metadata.CorrelationId = causationAndCorrelationId;
            expectedEvent.Metadata.ProcessId = command.Metadata.ProcessId;

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
                    new ProductReserved(id, 2)
                };
            }
        }
    }
}
