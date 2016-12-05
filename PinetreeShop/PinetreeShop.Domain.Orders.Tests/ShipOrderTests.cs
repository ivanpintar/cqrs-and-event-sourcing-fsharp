using System;
using Xunit;
using PinetreeShop.Domain.Orders.Events;
using System.Collections.Generic;
using PinetreeShop.Domain.Orders.Commands;
using PinetreeShop.Domain.Tests.Order.Exceptions;
using System.Linq;
using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.Domain.Shared.Types;

namespace PinetreeShop.Domain.Orders.Tests
{
    public class ShipOrderTests : OrderTestBase
    {
        Guid id = Guid.NewGuid();
        Guid basketId = Guid.NewGuid();
        Address shippingAddress = new Address { Country = "US", StateOrProvince = "CA", StreetAndNumber = "A2", ZipAndCity = "LA" };
        Guid causationAndCorrelationId = Guid.NewGuid();

        [Fact]
        public void When_ShipOrder_OrderShipped()
        {
            Given(InitialEvents);

            var command = new ShipOrder(id);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            When(command);

            var expectedEvent = new OrderShipped(id);
            expectedEvent.Metadata.CausationId = command.Metadata.CommandId;
            expectedEvent.Metadata.CorrelationId = causationAndCorrelationId;
            expectedEvent.Metadata.ProcessId = command.Metadata.ProcessId;

            Then(expectedEvent);
        }

        [Fact]
        public void When_ShipOrderCancelled_ThrowInvalidOrderStateException()
        {
            var events = InitialEvents.Take(2).ToList();
            events.Add(new OrderCancelled(id));
            Given(events.ToArray());
            WhenThrows<ShipOrder, InvalidOrderStateException>(new ShipOrder(id));
        }

        [Fact]
        public void When_ShipOrderCreated_ThrowInvalidOrderStateException()
        {
            var events = InitialEvents.Take(1).ToList();
            Given(events.ToArray());
            WhenThrows<ShipOrder, InvalidOrderStateException>(new ShipOrder(id));
        }

        private IEvent[] InitialEvents
        {
            get
            {
                return new IEvent[]
                {
                    new OrderCreated(id, basketId, shippingAddress),
                    new OrderLineAdded(id, OrderLines.First()),
                    new OrderReadyForShipping(id)
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
