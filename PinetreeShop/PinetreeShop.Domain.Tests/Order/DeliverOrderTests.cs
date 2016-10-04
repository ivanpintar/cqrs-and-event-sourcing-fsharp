using PinetreeShop.CQRS.Infrastructure.CommandsAndEvents;
using PinetreeShop.Domain.Orders.Commands;
using PinetreeShop.Domain.Orders.Events;
using PinetreeShop.Domain.Tests.Order.Exceptions;
using PinetreeShop.Domain.Types;
using System;
using System.Collections.Generic;
using Xunit;

namespace PinetreeShop.Domain.Tests.Order
{
    public class DeliverOrderTests : TestBase
    {
        Guid id = Guid.NewGuid();
        Guid basketId = Guid.NewGuid();
        Address shippingAddress = new Address { Country = "US", StateOrProvince = "CA", StreetAndNumber = "A2", ZipAndCity = "LA" };

        [Fact]
        public void When_DeliverOrder_OrderDelivered()
        {
            Given(InitialEvents);
            When(new DeliverOrder(id));
            Then(new OrderDelivered(id, shippingAddress));
        }

        [Fact]
        public void When_DeliverOrderCancelled_ThrowOrderCancelledException()
        {
            Given(
                new OrderCreated(id, basketId, OrderLines, shippingAddress), 
                new OrderCancelled(id));
            WhenThrows<InvalidOrderStateException>(new DeliverOrder(id));
        }

        private IEvent[] InitialEvents
        {
            get
            {
                return new IEvent[]
                {
                    new OrderCreated(id, basketId, OrderLines, shippingAddress),
                    new OrderShipped(id,shippingAddress)
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
