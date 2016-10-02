using PinetreeShop.Domain.Products.Commands;
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
        [Fact]
        [InlineData("One", 1.0)]
        [InlineData("Two", 2.0)]
        [InlineData("Three", 3.0)]
        public void When_ProductCreated_ProductIsCreatedWithTheGivenPrice(string name, decimal price)
        {
            Guid id = Guid.NewGuid();

            When(new CreateProduct(id, name, price));
        }
    }
}
