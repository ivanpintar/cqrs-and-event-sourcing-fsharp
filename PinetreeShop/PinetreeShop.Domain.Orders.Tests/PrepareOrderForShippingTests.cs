using PinetreeCQRS.Infrastructure.Events;
using PinetreeShop.Domain.Orders.Commands;
using PinetreeShop.Domain.Orders.Events;
using PinetreeShop.Domain.Shared.Types;
using PinetreeShop.Domain.Tests.Order.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PinetreeShop.Domain.Orders.Tests
{
    public class PrepareOrderForShippingTests : OrderTestBase
    {
        Guid id = Guid.NewGuid();
        Guid basketId = Guid.NewGuid();
        Guid productId = Guid.NewGuid();
        Guid causationAndCorrelationId = Guid.NewGuid();
        Address shippingAddress = new Address { Country = "US", StateOrProvince = "CA", StreetAndNumber = "A2", ZipAndCity = "LA" };

        [Fact]
        private void When_PrepareForShipping_OrderReadyForShipping()
        {
            Given(InitialEvents.ToArray());

            var command = new PrepareOrderForShipping(id);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            When(command);

            var expectedEvent = new OrderReadyForShipping(id);
            expectedEvent.Metadata.CausationId = command.Metadata.CommandId;
            expectedEvent.Metadata.CorrelationId = causationAndCorrelationId;
            expectedEvent.Metadata.ProcessId = command.Metadata.ProcessId;

            Then(expectedEvent);
        }

        [Fact]
        private void When_PrepareForShippingNoOrderLinesAdded_ThrowsInvalidOrderStateException()
        {
            Given(InitialEvents.Take(1).ToArray());

            var command = new PrepareOrderForShipping(id);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            WhenThrows<PrepareOrderForShipping, InvalidOrderStateException>(command);
        }

        [Fact]
        private void When_PrepareForShippingCancelled_ThrowsInvalidOrderStateException()
        {
            Given(InitialEvents, new OrderCancelled(id));

            var command = new PrepareOrderForShipping(id);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            WhenThrows<PrepareOrderForShipping, InvalidOrderStateException>(command);
        }

        [Fact]
        private void When_PrepareForShippingShipped_ThrowsInvalidOrderStateException()
        {
            Given(InitialEvents, new OrderReadyForShipping(id), new OrderShipped(id));

            var command = new PrepareOrderForShipping(id);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            WhenThrows<PrepareOrderForShipping, InvalidOrderStateException>(command);
        }

        [Fact]
        private void When_PrepareForShippingDelivered_ThrowsInvalidOrderStateException()
        {
            Given(InitialEvents, new OrderReadyForShipping(id), new OrderShipped(id), new OrderDelivered(id));

            var command = new PrepareOrderForShipping(id);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            WhenThrows<PrepareOrderForShipping, InvalidOrderStateException>(command);
        }

        private IEvent[] InitialEvents
        {
            get
            {
                return new IEvent[]
                {
                    new OrderCreated(id, basketId, shippingAddress),
                    new OrderLineAdded(id, OrderLines.First())
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
