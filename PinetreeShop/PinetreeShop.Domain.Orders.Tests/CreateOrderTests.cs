using PinetreeShop.Domain.Orders.Commands;
using System;
using Xunit;
using System.Collections.Generic;
using PinetreeShop.Domain.Orders.Events;
using PinetreeShop.Domain.Tests.Order.Exceptions;
using System.Linq;
using PinetreeShop.Domain.Shared.Types;
using PinetreeShop.Domain.Shared.Exceptions;

namespace PinetreeShop.Domain.Orders.Tests
{
    public class CreateOrderTests : OrderTestBase
    {
        Guid id = Guid.NewGuid();
        Guid basketId = Guid.NewGuid();
        Address shippingAddress = new Address { Country = "US", StateOrProvince = "CA", StreetAndNumber = "A2", ZipAndCity = "LA" };
        Guid productId = Guid.NewGuid();
        Guid causationAndCorrelationId = Guid.NewGuid();

        [Fact]
        public void When_CreateOrder_OrderCreated()
        {
            var command = new CreateOrder(id, basketId, shippingAddress, causationAndCorrelationId);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            When(command);

            var expectedEvent = new OrderCreated(id, basketId, causationAndCorrelationId, shippingAddress);
            expectedEvent.Metadata.CausationId = command.Metadata.CommandId;
            expectedEvent.Metadata.CorrelationId = causationAndCorrelationId;

            Then(expectedEvent);
        }

        [Fact]
        public void When_CreateOrderWithNoShippingAddress_ThrowParameterNullException()
        {
            WhenThrows<CreateOrder, ParameterNullException>(new CreateOrder(id, basketId, null, causationAndCorrelationId));
        }
        
        [Fact]
        public void When_CreateOrderWithSameGuid_ThrowAggregateExistsException()
        {
            Given(new OrderCreated(id, basketId, causationAndCorrelationId, shippingAddress));
            WhenThrows<CreateOrder, AggregateExistsException>(new CreateOrder(id, basketId, shippingAddress, causationAndCorrelationId));
        }
    }
}
