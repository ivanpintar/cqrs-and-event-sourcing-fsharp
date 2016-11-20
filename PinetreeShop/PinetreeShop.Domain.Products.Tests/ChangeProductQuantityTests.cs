using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.Domain.Products.Commands;
using PinetreeShop.Domain.Products.Events;
using PinetreeShop.Domain.Products.Exceptions;
using System;
using Xunit;

namespace PinetreeShop.Domain.Products.Tests
{
    public class ChangeProductQuantityTests : ProductTestBase
    {
        Guid id = Guid.NewGuid();
        Guid causationAndCorrelationId = Guid.NewGuid();

        [Fact]
        public void When_SetProductQuantity_ProductQuantityChanged()
        {
            Given(new ProductCreated(id, "Test Product", 2));

            var command = new SetProductQuantity(id, 5);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            When(command);

            var expectedEvent = new ProductQuantityChanged(id, 5);
            expectedEvent.Metadata.CausationId = command.Metadata.CommandId;
            expectedEvent.Metadata.CorrelationId = causationAndCorrelationId;

            Then(expectedEvent);
        }

        [Fact]
        public void When_SetProductQuantitySame_NothingHappens()
        {
            Given(
                new ProductCreated(id, "Test Product", 2), 
                new ProductQuantityChanged(id, 2));

            var command = new SetProductQuantity(id, 2);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            When(command);           

            Then(new IEvent[] { });
        }

        [Fact]
        public void When_ChangeProductQuantityBelowZero_ThrowQuantityChangeException()
        {
            Given(new ProductCreated(id, "Test Product", 2), new ProductQuantityChanged(id, 2));
            WhenThrows<SetProductQuantity, QuantityChangeException>(new SetProductQuantity(id, -4));
        }
    }
}
