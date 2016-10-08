using System;
using Xunit;
using PinetreeShop.Domain.Orders.Events;
using PinetreeShop.Domain.Types;
using System.Collections.Generic;
using PinetreeShop.Domain.Orders.Commands;
using PinetreeShop.Domain.Tests.Order.Exceptions;
using System.Linq;
using PinetreeShop.CQRS.Infrastructure.Events;

namespace PinetreeShop.Domain.Tests.Order
{
    public class ShipOrderTests : TestBase
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
            command.Metadata.CausationId = command.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            When(command);

            var expectedEvent = new OrderShipped(id, shippingAddress);
            expectedEvent.Metadata.CausationId = command.Metadata.CommandId;
            expectedEvent.Metadata.CorrelationId = causationAndCorrelationId;

            Then(expectedEvent);
        }

        [Fact]
        public void When_ShipOrderCancelled_ThrowOrderCancelledException()
        {
            var events = InitialEvents.ToList();
            events.Add(new OrderCancelled(id));
            Given(events.ToArray());
            WhenThrows<InvalidOrderStateException>(new ShipOrder(id));
        }

        private IEvent[] InitialEvents
        {
            get
            {
                return new IEvent[]
                {
                    new OrderCreated(id, basketId, OrderLines, shippingAddress)
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
