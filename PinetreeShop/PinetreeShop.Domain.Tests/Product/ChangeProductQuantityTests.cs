using PinetreeShop.Domain.Products.Commands;
using PinetreeShop.Domain.Products.Events;
using PinetreeShop.Domain.Products.Exceptions;
using System;
using Xunit;

namespace PinetreeShop.Domain.Tests.Product
{
    public class ChangeProductQuantityTests : TestBase
    {
        Guid id = Guid.NewGuid();
        Guid causationAndCorrelationId = Guid.NewGuid();

        [Fact]
        public void When_ChangeProductQuantity_ProductQuantityChanged()
        {
            Given(new ProductCreated(id, "Test Product", 2));

            var command = new ChangeProductQuantity(id, 2);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            When(command);

            var expectedEvent = new ProductQuantityChanged (id, 2);
            expectedEvent.Metadata.CausationId = command.Metadata.CommandId;
            expectedEvent.Metadata.CorrelationId = causationAndCorrelationId;

            Then(expectedEvent);
        }

        [Fact]
        public void When_ChangeProductQuantityBelowZero_ThrowQuantityChangeException()
        {
            Given(new ProductCreated(id, "Test Product", 2), new ProductQuantityChanged(id, 2));
            WhenThrows<QuantityChangeException>(new ChangeProductQuantity(id, -4));
        }
    }
}
