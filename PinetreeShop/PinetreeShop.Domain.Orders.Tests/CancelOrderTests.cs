using PinetreeCQRS.Infrastructure.Events;
using PinetreeShop.Domain.Orders.Commands;
using PinetreeShop.Domain.Orders.Events;
using PinetreeShop.Domain.Shared.Types;
using PinetreeShop.Domain.Tests.Order.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PinetreeShop.Domain.Orders.Tests
{
    public class CancelOrderTests : OrderTestBase
    {
        Guid id = Guid.NewGuid();
        Guid basketId = Guid.NewGuid();
        Guid productId = Guid.NewGuid();
        Guid causationAndCorrelationId = Guid.NewGuid();
        Address shippingAddress = new Address { Country = "US", StateOrProvince = "CA", StreetAndNumber = "A2", ZipAndCity = "LA" };

        [Fact]
        public void When_CancelOrder_OrderCancelled()
        {
            Given(InitialEvents.Take(2).ToArray());

            var command = new CancelOrder(id);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            When(command);

            var expectedEvent = new OrderCancelled(id);
            expectedEvent.Metadata.CausationId = command.Metadata.CommandId;
            expectedEvent.Metadata.CorrelationId = causationAndCorrelationId;
            expectedEvent.Metadata.ProcessId = command.Metadata.ProcessId;

            Then(expectedEvent);
        }

        [Fact]
        public void When_CancelOrderWhenShipped_CancelOrderFailed()
        {
            Given(InitialEvents.Take(3).ToArray());

            var command = new CancelOrder(id);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            WhenThrows<CancelOrder, InvalidOrderStateException>(command);
        }

        [Fact]
        public void When_CancelOrderWhenDelivered_ThrowsInvalidOrderStateException()
        {
            Given(InitialEvents);

            var command = new CancelOrder(id);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            WhenThrows<CancelOrder, InvalidOrderStateException>(command);
        }

        private IEvent[] InitialEvents
        {
            get
            {
                return new IEvent[]
                {
                    new OrderCreated(id, basketId, shippingAddress),
                    new OrderReadyForShipping(id),
                    new OrderShipped(id),
                    new OrderDelivered(id)
                };
            }
        }

        public IEnumerable<OrderLine> OrderLines
        {
            get
            {
                return new List<OrderLine>
                {
                    new OrderLine
                    {
                        ProductId = productId,
                        ProductName = "Test Product",
                        Price = 2,
                        Quantity = 2
                    }
                };
            }
        }
    }
}
