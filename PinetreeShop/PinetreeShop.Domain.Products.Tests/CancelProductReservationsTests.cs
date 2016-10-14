using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.Domain.Products.Commands;
using PinetreeShop.Domain.Products.Events;
using System;
using Xunit;

namespace PinetreeShop.Domain.Products.Tests
{
    public class CancelProductReservationsTests : ProductTestBase
    {
        Guid id = Guid.NewGuid();
        Guid basketId = Guid.NewGuid();
        Guid causationAndCorrelationId = Guid.NewGuid();

        [Fact]
        public void When_CancelProductResevation_ProductReservationCancelled()
        {

            Given(InitialEvents);

            var command = new CancelProductReservation(id, 2);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            When(command);

            var expectedEvent = new ProductReservationCancelled(id, 2);
            expectedEvent.Metadata.CausationId = command.Metadata.CommandId;
            expectedEvent.Metadata.CorrelationId = causationAndCorrelationId;

            Then(expectedEvent);
        }

        [Fact]
        public void When_CancelProductReservationLessThanReserved_ProductReservationCanceled()
        {
            Given(InitialEvents);

            var command = new CancelProductReservation(id, 10);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            When(command);

            var expectedEvent = new ProductReservationCancelled(id, 3);
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
                    new ProductReserved(id, basketId, 3)
                };
            }
        }
    }
}
