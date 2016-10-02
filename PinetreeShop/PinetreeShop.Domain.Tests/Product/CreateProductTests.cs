using PinetreeShop.Domain.Products.Commands;
using PinetreeShop.Domain.Products.Events;
using PinetreeShop.Domain.Products.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PinetreeShop.Domain.Tests.Product
{
    public class CreateProductTests : TestBase
    {
        [Theory]
        [InlineData("One", 1)]
        [InlineData("Two", 2)]
        [InlineData("Three", 3)]
        public void When_ProductCreated_ProductIsCreatedWithTheGivenPrice(string name, int price)
        {
            Guid id = Guid.NewGuid();

            When(new CreateProduct(id, name, price));
            Then(new ProductCreated(id, name, price));
        }

        [Fact]
        public void When_CreatingProductWithSameGuid_ThrowProducExistsException()
        {
            Guid id = Guid.NewGuid();

            Given(new ProductCreated(id, "One", 1));
            WhenTrows<ProductExistsException>(new CreateProduct(id, "One", 1));
        }
    }
}
