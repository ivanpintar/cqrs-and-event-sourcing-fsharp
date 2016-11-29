using PinetreeShop.CQRS.Infrastructure.Events;
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
    public class DeliverOrderTests : OrderTestBase
    {
        Guid id = Guid.NewGuid();
        Guid basketId = Guid.NewGuid();
        Address shippingAddress = new Address { Country = "US", StateOrProvince = "CA", StreetAndNumber = "A2", ZipAndCity = "LA" };
        Guid causationAndCorrelationId = Guid.NewGuid();

        [Fact]
        public void When_DeliverOrder_OrderDelivered()
        {
            Given(InitialEvents);

            var command = new DeliverOrder(id);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            When(command);

            var expectedEvent = new OrderDelivered(id);
            expectedEvent.Metadata.CausationId = command.Metadata.CommandId;
            expectedEvent.Metadata.CorrelationId = causationAndCorrelationId;

            Then(expectedEvent);
        }

        [Fact]
        public void When_DeliverOrderCancelled_ThrowOrderCancelledException()
        {
            Given(
                new OrderCreated(id, basketId, causationAndCorrelationId, shippingAddress),
                new OrderLineAdded(id, OrderLines.First()),
                new OrderReadyForShipping(id),
                new OrderCancelled(id));
            WhenThrows<DeliverOrder, InvalidOrderStateException>(new DeliverOrder(id));
        }

        private IEvent[] InitialEvents
        {
            get
            {
                return new IEvent[]
                {
                    new OrderCreated(id, basketId, causationAndCorrelationId, shippingAddress),
                    new OrderLineAdded(id, OrderLines.First()),
                    new OrderReadyForShipping(id),
                    new OrderShipped(id)
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
                        ProductId = Guid.NewGuid(),
                        ProductName = "Test Product",
                        Price = 2,
                        Quantity = 2
                    }
                };
            }
        }
    }
}
