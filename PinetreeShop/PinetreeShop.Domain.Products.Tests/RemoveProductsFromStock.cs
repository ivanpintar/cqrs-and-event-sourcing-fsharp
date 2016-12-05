using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.Domain.Products.Commands;
using PinetreeShop.Domain.Products.Events;
using PinetreeShop.Domain.Products.Exceptions;
using System;
using Xunit;

namespace PinetreeShop.Domain.Products.Tests
{
    public class RemoveProductFromStockTests : ProductTestBase
    {
        Guid id = Guid.NewGuid();
        Guid causationAndCorrelationId = Guid.NewGuid();

        [Fact]
        public void When_RemoveProductFromStock_ProductQuantityChanged()
        {
            Given(
                new ProductCreated(id, "Test Product", 5),
                new ProductQuantityChanged(id, 5));

            var command = new RemoveProductFromStock(id, 3);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            When(command);

            var expectedEvent = new ProductQuantityChanged(id, -3);
            expectedEvent.Metadata.CausationId = command.Metadata.CommandId;
            expectedEvent.Metadata.CorrelationId = causationAndCorrelationId;
            expectedEvent.Metadata.ProcessId = command.Metadata.ProcessId;

            Then(expectedEvent);
        }

        [Fact]
        public void When_RemoveProductFromStockZero_NothingHappens()
        {
            Given(
                new ProductCreated(id, "Test Product", 2),
                new ProductQuantityChanged(id, 5));

            var command = new RemoveProductFromStock(id, 0);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            When(command);

            Then(new IEvent[] { });
        }

        [Fact]
        public void When_RemoveProductFromStockZero_ThrowQuantityChangeException()
        {
            Given(new ProductCreated(id, "Test Product", 2), new ProductQuantityChanged(id, 2));

            WhenThrows<RemoveProductFromStock, QuantityChangeException>(new RemoveProductFromStock(id, 4));
        }
    }
}
