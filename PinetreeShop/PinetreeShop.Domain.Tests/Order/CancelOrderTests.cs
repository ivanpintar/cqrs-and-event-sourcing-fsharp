using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.Domain.Orders.Commands;
using PinetreeShop.Domain.Orders.Events;
using PinetreeShop.Domain.Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PinetreeShop.Domain.Tests.Order
{
    public class CancelOrderTests : TestBase
    {
        Guid id = Guid.NewGuid();
        Guid basketId = Guid.NewGuid();
        Guid productId = Guid.NewGuid();
        Guid causationAndCorrelationId = Guid.NewGuid();
        Address shippingAddress = new Address { Country = "US", StateOrProvince = "CA", StreetAndNumber = "A2", ZipAndCity = "LA" };

        [Fact]
        public void When_CancelOrder_OrderCancelled()
        {
            Given(InitialEvents.Take(1).ToArray());

            var command = new CancelOrder(id);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            When(command);

            var expectedEvent = new OrderCancelled(id);
            expectedEvent.Metadata.CausationId = command.Metadata.CommandId;
            expectedEvent.Metadata.CorrelationId = causationAndCorrelationId;

            Then(expectedEvent);
        }

        [Fact]
        public void When_CancelOrderWhenShipped_CancelOrderFailed()
        {
            Given(InitialEvents.Take(2).ToArray());

            var command = new CancelOrder(id);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            When(command);

            var expectedEvent = new CancelOrderFailed(id, CancelOrderFailed.OrderShipped);
            expectedEvent.Metadata.CausationId = command.Metadata.CommandId;
            expectedEvent.Metadata.CorrelationId = causationAndCorrelationId;

            Then(expectedEvent);
        }

        [Fact]
        public void When_CancelOrderWhenDelivered_CancelOrderFailed()
        {
            Given(InitialEvents);

            var command = new CancelOrder(id);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            When(command);

            var expectedEvent = new CancelOrderFailed(id, CancelOrderFailed.OrderDelivered);
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
                    new OrderCreated(id, basketId, OrderLines, shippingAddress),
                    new OrderShipped(id,shippingAddress),
                    new OrderDelivered(id,shippingAddress)
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
