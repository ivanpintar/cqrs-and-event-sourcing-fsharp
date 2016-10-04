using PinetreeShop.CQRS.Infrastructure.CommandsAndEvents;
using PinetreeShop.Domain.Baskets.Commands;
using PinetreeShop.Domain.Baskets.Events;
using System;
using System.Linq;
using Xunit;

namespace PinetreeShop.Domain.Tests.Basket
{
    public class AddProductTests :TestBase
    {
        Guid id = Guid.NewGuid();
        Guid productId = Guid.NewGuid();

        [Fact]
        public void When_AddProduct_ProductAdded()
        {
            Given(InitialEvents);
            When(new AddProduct(id, productId, "Test Product", 2, 10));
            Then(new ProductAdded(id, productId, "Test Product", 2, 10));
        }

        [Fact]
        public void When_RevertAddProduct_AddProductReverted()
        {
            var events = InitialEvents.ToList();
            events.Add(new ProductAdded(id, productId, "Test Product", 2, 10));
            Given(events.ToArray());
            When(new RevertAddProduct(id, productId, 10, "reason"));
            Then(new AddProductReverted(id, productId, 10, "reason"));
        }

        private IEvent[] InitialEvents
        {
            get
            {
                return new IEvent[]
                {
                    new BasketCreated(id)
                };
            }
        }
    }
}
