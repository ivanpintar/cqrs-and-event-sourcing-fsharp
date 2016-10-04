using PinetreeShop.Domain.Orders.Commands;
using System;
using Xunit;
using PinetreeShop.Domain.Types;
using System.Collections.Generic;
using PinetreeShop.Domain.Orders.Events;
using PinetreeShop.Domain.Exceptions;
using PinetreeShop.Domain.Products.Commands;
using PinetreeShop.Domain.Tests.Order.Exceptions;
using System.Linq;

namespace PinetreeShop.Domain.Tests.Order
{
    public class CreateOrderTests : TestBase
    {
        Guid id = Guid.NewGuid();
        Guid basketId = Guid.NewGuid();
        Address shippingAddress = new Address { Country = "US", StateOrProvince = "CA", StreetAndNumber = "A2", ZipAndCity = "LA" };

        [Fact]
        public void When_CreateOrder_OrderCreated()
        {
            When(new CreateOrder(id, basketId, OrderLines, shippingAddress));
            Then(new OrderCreated(id, basketId, OrderLines, shippingAddress));
        }

        [Fact]
        public void When_CreateOrderWithNoLines_ThrowEmptyOrderLinesException()
        {
            WhenThrows<EmptyOrderLinesException>(new CreateOrder(id, basketId, Enumerable.Empty<OrderLine>(), shippingAddress));
        }

        [Fact]
        public void When_CreateOrderWithNoShippingAddress_ThrowParameterNullException()
        {
            WhenThrows<ParameterNullException>(new CreateOrder(id, basketId, OrderLines, null));
        }
        
        [Fact]
        public void When_CreateOrderWithSameGuid_ThrowAggregateExistsException()
        {
            Given(new OrderCreated(id, basketId, OrderLines, shippingAddress));
            WhenThrows<AggregateExistsException>(new CreateOrder(id, basketId, OrderLines, shippingAddress));
        }

        private IEnumerable<OrderLine> OrderLines
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
