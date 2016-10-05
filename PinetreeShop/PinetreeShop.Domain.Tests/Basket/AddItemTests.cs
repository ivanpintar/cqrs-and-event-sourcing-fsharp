using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.Domain.Baskets.Commands;
using PinetreeShop.Domain.Baskets.Events;
using System;
using System.Linq;
using Xunit;

namespace PinetreeShop.Domain.Tests.Basket
{
    public class AddItemTests :TestBase
    {
        Guid id = Guid.NewGuid();
        Guid productId = Guid.NewGuid();

        [Fact]
        public void When_TryAddItem_AddItemTried()
        {
            Given(InitialEvents);
            When(new TryAddItemToBasket(id, productId, "Test Item", 2, 10));
            Then(new BasketAddItemTried(id, productId, "Test Item", 2, 10));
        }

        [Fact]
        public void When_ConfirmAddItem_AddItemReverted()
        {
            var events = InitialEvents.ToList();
            events.Add(new BasketAddItemTried(id, productId, "Test Item", 2, 10));
            Given(events.ToArray());
            When(new ConfirmAddItemToBasket(id, productId, 10));
            Then(new BasketAddItemConfirmed(id, productId, 10));
        }

        [Fact]
        public void When_RevertAddItem_AddItemReverted()
        {
            var events = InitialEvents.ToList();
            events.Add(new BasketAddItemTried(id, productId, "Test Item", 2, 10));
            Given(events.ToArray());
            When(new RevertAddItemToBasket(id, productId, 10, "reason"));
            Then(new BasketAddItemReverted(id, productId, 10, "reason"));
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
