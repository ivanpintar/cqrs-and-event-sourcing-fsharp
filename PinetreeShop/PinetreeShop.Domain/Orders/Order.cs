using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.Domain.Exceptions;
using PinetreeShop.Domain.Orders.Events;
using PinetreeShop.Domain.Tests.Order.Exceptions;
using PinetreeShop.Domain.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PinetreeShop.Domain.Orders
{
    public class Order : AggregateBase
    {
        private enum OrderState { Pending, Shipped, Cancelled, Delivered };
        private OrderState _state = OrderState.Pending;
        public IEnumerable<OrderLine> _orderLines = new List<OrderLine>();
        public Guid BasketId { get; private set; }
        public Address ShippingAddress { get; private set; }

        public Order()
        {
            RegisterEventHandler<OrderCreated>(Apply);
            RegisterEventHandler<OrderCancelled>(Apply);
            RegisterEventHandler<OrderShipped>(Apply);
            RegisterEventHandler<OrderDelivered>(Apply);
        }

        public Order(Guid orderId, Guid baskedId, IEnumerable<OrderLine> orderLines, Address shippingAddress) : base()
        {
            if (orderLines.Count() == 0) throw new EmptyOrderLinesException(orderId, "Can't create an order without empty lines");
            if (shippingAddress == null) throw new ParameterNullException(orderId, "shippingAddress");

            RaiseEvent(new OrderCreated(orderId, baskedId, orderLines, shippingAddress));
        }

        private void Apply(OrderCreated evt)
        {
            Id = evt.AggregateId;
            BasketId = evt.BasketId;
            ShippingAddress = evt.ShippingAddress;
            _orderLines = evt.Lines;
        }

        internal void Cancel(Guid aggregateId)
        {
            switch (_state)
            {
                case OrderState.Shipped:
                    RaiseEvent(new CancelOrderFailed(aggregateId, CancelOrderFailed.OrderShipped));
                    break;
                case OrderState.Delivered:
                    RaiseEvent(new CancelOrderFailed(aggregateId, CancelOrderFailed.OrderDelivered));
                    break;
                default:
                    RaiseEvent(new OrderCancelled(aggregateId));
                    break;
            }
        }

        internal void Ship(Guid aggregateId)
        {
            if (_state != OrderState.Pending) throw new InvalidOrderStateException(aggregateId, $"State should be {OrderState.Pending} but is {_state}");

            RaiseEvent(new OrderShipped(aggregateId, ShippingAddress));
        }

        internal void Deliver(Guid aggregateId)
        {
            if (_state != OrderState.Shipped) throw new InvalidOrderStateException(aggregateId, $"State should be {OrderState.Shipped} but is {_state}");

            RaiseEvent(new OrderDelivered(aggregateId, ShippingAddress));
        }

        private void Apply(OrderCancelled evt)
        {
            Id = evt.AggregateId;
            _state = OrderState.Cancelled;
        }

        private void Apply(OrderShipped evt)
        {
            Id = evt.AggregateId;
            _state = OrderState.Shipped;
        }

        private void Apply(OrderDelivered evt)
        {
            Id = evt.AggregateId;
            _state = OrderState.Delivered;
        }

        internal static IAggregate Create(Guid orderId, Guid basketId, IEnumerable<OrderLine> lines, Address shippingAddress)
        {
            return new Order(orderId, basketId, lines, shippingAddress);
        }
    }
}
