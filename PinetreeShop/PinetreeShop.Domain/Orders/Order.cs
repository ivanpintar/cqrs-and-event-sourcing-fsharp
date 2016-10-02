using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.Domain.Orders.Events;
using PinetreeShop.Domain.Types;
using System;
using System.Collections.Generic;

namespace PinetreeShop.Domain.Orders
{
    public class Order : AggregateBase
    {
        private enum OrderState { Pending, Shipped, Cancelled, Delivered };
        private OrderState _state = OrderState.Pending;
        public List<OrderLine> _orderLines = new List<OrderLine>();

        public Order()
        {
            RegisterTransition<OrderCreated>(Apply);
            RegisterTransition<OrderCancelled>(Apply);
            RegisterTransition<OrderShipped>(Apply);
            RegisterTransition<OrderDelivered>(Apply);
        }

        public Order(Guid baskedId, IEnumerable<OrderLine> orderLines) : base()
        {

        }

        private void Apply(OrderCreated evt)
        {
            throw new NotImplementedException();
        }

        private void Apply(OrderCancelled evt)
        {
            throw new NotImplementedException();
        }

        private void Apply(OrderShipped evt)
        {
            throw new NotImplementedException();
        }

        private void Apply(OrderDelivered evt)
        {
            throw new NotImplementedException();
        }
    }
}
