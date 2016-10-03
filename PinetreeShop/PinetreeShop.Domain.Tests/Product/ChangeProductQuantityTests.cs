using PinetreeShop.Domain.Products.Commands;
using PinetreeShop.Domain.Products.Events;
using PinetreeShop.Domain.Products.Exceptions;
using System;
using Xunit;

namespace PinetreeShop.Domain.Tests.Product
{
    public class ChangeProductQuantityTests : TestBase
    {
        [Fact]
        public void When_ChangeProductQuantity_ProductQuantityChanged()
        {
            Guid id = Guid.NewGuid();

            Given(new ProductCreated(id, "quantity test", 2));
            When(new ChangeProductQuantity(id, 2));
            Then(new ProductQuantityChanged(id, 2));
        }

        [Fact]
        public void When_ChangeProductQuantityBelowZero_ThrowQuantityChangeException()
        {
            Guid id = Guid.NewGuid();

            Given(new ProductCreated(id, "quantity test", 2), new ProductQuantityChanged(id, 2));
            WhenTrows<QuantityChangeException>(new ChangeProductQuantity(id, -4));
        }
    }
}
