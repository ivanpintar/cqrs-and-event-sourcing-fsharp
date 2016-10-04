using PinetreeShop.CQRS.Infrastructure.CommandsAndEvents;
using PinetreeShop.Domain.Baskets.Commands;
using PinetreeShop.Domain.Baskets.Events;
using PinetreeShop.Domain.Baskets.Exceptions;
using System;
using Xunit;

namespace PinetreeShop.Domain.Tests.Basket
{
    public class RemoveProductTests : TestBase
    {
        Guid id = Guid.NewGuid();
        Guid productId = Guid.NewGuid();

        [Fact]
        public void When_RemoveProduct_ProductRemoved()
        {
            Given(InitialEvents);
            When(new RemoveProduct(id, productId, 5));
            Then(new ProductRemoved(id, productId, 5));
        }

        [Fact]
        public void When_RemoveProductBelowZero_ProductRemoved()
        {
            Given(InitialEvents);
            When(new RemoveProduct(id, productId, 15));
            Then(new ProductRemoved(id, productId, 10));
        }
        

        private IEvent[] InitialEvents
        {
            get
            {
                return new IEvent[]
                {
                    new BasketCreated(id),
                    new ProductAdded(id, productId, "Test Product", 2, 10)
                };
            }
        }
    }
}
