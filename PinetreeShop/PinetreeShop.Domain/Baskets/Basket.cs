using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.Domain.Baskets.Events;
using PinetreeShop.Domain.Types;
using System;
using System.Collections.Generic;

namespace PinetreeShop.Domain.Baskets
{
    public class Basket : AggregateBase
    {
        private enum BasketState { Pending, CheckedOut };
        private BasketState _state = BasketState.Pending;
        private List<OrderLine> _orderLines = new List<OrderLine>();
        
        private Basket(Guid id) : this()
        {
        }

        public Basket()
        {

            RegisterTransition<BasketCreated>(Apply);
            RegisterTransition<ProductAdded>(Apply);
            RegisterTransition<ProductRemoved>(Apply);
            RegisterTransition<CheckedOut>(Apply);
        }

        private void Apply(BasketCreated evt)
        {
            throw new NotImplementedException();
        }

        private void Apply(ProductAdded evt)
        {
            throw new NotImplementedException();
        }

        private void Apply(ProductRemoved evt)
        {
            throw new NotImplementedException();
        }

        private void Apply(CheckedOut evt)
        {
            throw new NotImplementedException();
        }
    }
}
