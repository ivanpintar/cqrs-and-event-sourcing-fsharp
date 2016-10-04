using PinetreeShop.Domain.Exceptions;
using PinetreeShop.Domain.Products.Commands;
using PinetreeShop.Domain.Products.Events;
using PinetreeShop.Domain.Products.Exceptions;
using System;
using Xunit;

namespace PinetreeShop.Domain.Tests.Product
{
    public class CreateProductTests : TestBase
    {
        Guid id = Guid.NewGuid();

        [Fact]
        public void When_CreateProduct_ProductCreated()
        {
            When(new CreateProduct(id, "Test Product", 2));
            Then(new ProductCreated(id, "Test Product", 2));
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
