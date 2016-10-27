using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.Domain.Products.Commands;
using PinetreeShop.Domain.Products.Events;
using PinetreeShop.Domain.Products.Exceptions;
using System;
using Xunit;

namespace PinetreeShop.Domain.Products.Tests
{
    public class AddProductToStockTests : ProductTestBase
    {
        Guid id = Guid.NewGuid();
        Guid causationAndCorrelationId = Guid.NewGuid();

        [Fact]
        public void When_AddProductToStock_ProductQuantityChanged()
        {
            Given(new ProductCreated(id, "Test Product", 2));

            var command = new AddProductToStock(id, 5);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            When(command);

            var expectedEvent = new ProductQuantityChanged(id, 5);
            expectedEvent.Metadata.CausationId = command.Metadata.CommandId;
            expectedEvent.Metadata.CorrelationId = causationAndCorrelationId;

            Then(expectedEvent);
        }

        [Fact]
        public void When_AddProductToStockZero_NothingHappens()
        {
            Given(new ProductCreated(id, "Test Product", 2));

            var command = new AddProductToStock(id, 0);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            When(command);

            Then(new IEvent[] { });
        }

        [Fact]
        public void When_AddQuantityBelowZero_ThrowQuantityChangeException()
        {
            Given(new ProductCreated(id, "Test Product", 2), new ProductQuantityChanged(id, 2));
            WhenThrows<AddProductToStock, QuantityChangeException>(new AddProductToStock(id, -4));
        }
    }
}
