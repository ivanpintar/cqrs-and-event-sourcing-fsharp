using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.Domain.Baskets.Commands;
using PinetreeShop.Domain.Baskets.Events;
using PinetreeShop.Domain.Baskets.Exceptions;
using System;
using Xunit;

namespace PinetreeShop.Domain.Tests.Basket
{
    public class RemoveItemTests : TestBase
    {
        Guid id = Guid.NewGuid();
        Guid productId = Guid.NewGuid();

        [Fact]
        public void When_RemoveItem_ItemRemoved()
        {
            Given(InitialEvents);
            When(new RemoveItemFromBasket(id, productId, 5));
            Then(new BasketItemRemoved(id, productId, 5));
        }

        [Fact]
        public void When_RemoveItemBelowZero_ItemRemoved()
        {
            Given(InitialEvents);
            When(new RemoveItemFromBasket(id, productId, 15));
            Then(new BasketItemRemoved(id, productId, 10));
        }
        

        private IEvent[] InitialEvents
        {
            get
            {
                return new IEvent[]
                {
                    new BasketCreated(id),
                    new BasketAddItemTried(id, productId, "Test Item", 2, 10)
                };
            }
        }
    }
}
