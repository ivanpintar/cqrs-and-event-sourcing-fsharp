using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.Domain.Baskets.Events;
using PinetreeShop.Domain.Types;
using System;
using System.Collections.Generic;

namespace PinetreeShop.Domain.Baskets
{
    public class Basket : AggregateBase
    {
        public enum BasketState { Pending, Cancelled, CheckedOut };
        public BasketState State { get; private set; }

        private List<OrderLine> _orderLines = new List<OrderLine>();
        public IEnumerable<OrderLine> OrderLines { get { return _orderLines; } }

        private Basket(Guid id) : this()
        {   
        }

        public Basket()
        {
            State = BasketState.Pending;

            RegisterTransition<BasketCreated>(Apply);
            RegisterTransition<ProductAdded>(Apply);
            RegisterTransition<ProductRemoved>(Apply);
            RegisterTransition<Cancelled>(Apply);
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

        private void Apply(Cancelled evt)
        {
            throw new NotImplementedException();
        }

        private void Apply(CheckedOut evt)
        {
            throw new NotImplementedException();
        }
    }
}
