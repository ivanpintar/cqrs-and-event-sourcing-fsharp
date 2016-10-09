using PinetreeShop.Domain.Exceptions;
using PinetreeShop.Domain.Products.Commands;
using PinetreeShop.Domain.Products.Events;
using PinetreeShop.Domain.Products.Exceptions;
using System;
using Xunit;

namespace PinetreeShop.Domain.Tests.Product
{
    public class CreateProductTests : AggregateTestBase
    {
        Guid id = Guid.NewGuid();
        Guid causationAndCorrelationId = Guid.NewGuid();

        [Fact]
        public void When_CreateProduct_ProductCreated()
        {
            var command = new CreateProduct(id, "Test Product", 2);
            command.Metadata.CausationId = command.Metadata.CommandId;
            command.Metadata.CorrelationId = causationAndCorrelationId;

            When(command);

            var expectedEvent = new ProductCreated(id, "Test Product", 2);
            expectedEvent.Metadata.CausationId = command.Metadata.CommandId;
            expectedEvent.Metadata.CorrelationId = causationAndCorrelationId;

            Then(expectedEvent);
        }

        [Fact]
        public void When_CreateProductWithNegativePrice_ProductCreated()
        {
            WhenThrows<ProductCreationException>(new CreateProduct(id, "Test Product", -2));
        }

        [Fact]
        public void When_CreateProductWithSameGuid_ThrowAggregateExistsException()
        {
            Given(new ProductCreated(id, "Test Product", 1));
            WhenThrows<AggregateExistsException>(new CreateProduct(id, "Test Product", 1));
        }
    }
}
