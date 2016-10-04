using PinetreeShop.Domain.Baskets.Commands;
using PinetreeShop.Domain.Baskets.Events;
using PinetreeShop.Domain.Exceptions;
using System;
using Xunit;

namespace PinetreeShop.Domain.Tests.Basket
{
    public class CreateBasketTests : TestBase
    {
        private Guid id = Guid.NewGuid();

        [Fact]
        public void When_CreateBasket_BasketCreated()
        {
            When(new CreateBasket(id));
            Then(new BasketCreated(id));
        }


        [Fact]
        public void When_CreateBasketWithSameGuid_ThrowAggregateExistsException()
        {
            Given(new BasketCreated(id));
            WhenThrows<AggregateExistsException>(new CreateBasket(id));
        }

    }
}
