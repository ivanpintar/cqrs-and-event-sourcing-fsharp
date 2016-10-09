using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.Domain.Exceptions;
using PinetreeShop.Domain.Orders.Commands;
using PinetreeShop.Domain.Orders.Events;
using PinetreeShop.Domain.Tests.Order.Exceptions;
using PinetreeShop.Domain.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PinetreeShop.Domain.Orders
{
    public class OrderAggregate : AggregateBase
    {
        private enum OrderState { Pending, Shipped, Cancelled, Delivered };
        private OrderState _state;
        private IEnumerable<OrderLine> _orderLines = new List<OrderLine>();
        private Guid _basketId { get; set; }
        private Address _shippingAddress { get; set; }

        public OrderAggregate()
        {
            RegisterEventHandler<OrderCreated>(Apply);
            RegisterEventHandler<OrderCancelled>(Apply);
            RegisterEventHandler<OrderShipped>(Apply);
            RegisterEventHandler<OrderDelivered>(Apply);
        }

        public OrderAggregate(CreateOrder cmd) : base()
        {
            var orderId = cmd.AggregateId;
            var basketId = cmd.BasketId;
            var orderLines = cmd.Lines;
            var shippingAddress = cmd.ShippingAddress;

            if (orderLines.Count() == 0) throw new EmptyOrderLinesException(orderId, "Can't create an order without empty lines");
            if (shippingAddress == null) throw new ParameterNullException(orderId, "shippingAddress");

            RaiseEvent(new OrderCreated(orderId, basketId, orderLines, shippingAddress));
        }

        private void Apply(OrderCreated evt)
        {
            _state = OrderState.Pending;
            AggregateId = evt.AggregateId;
            _basketId = evt.BasketId;
            _shippingAddress = evt.ShippingAddress;
            _orderLines = evt.Lines;
        }

        internal void Cancel(CancelOrder cmd)
        {
            switch (_state)
            {
                case OrderState.Shipped:
                    RaiseEvent(new CancelOrderFailed(cmd.AggregateId, CancelOrderFailed.OrderShipped));
                    break;
                case OrderState.Delivered:
                    RaiseEvent(new CancelOrderFailed(cmd.AggregateId, CancelOrderFailed.OrderDelivered));
                    break;
                default:
                    RaiseEvent(new OrderCancelled(cmd.AggregateId));
                    break;
            }
        }

        internal void Ship(ShipOrder cmd)
        {
            if (_state != OrderState.Pending) throw new InvalidOrderStateException(cmd.AggregateId, $"State should be {OrderState.Pending} but is {_state}");

            RaiseEvent(new OrderShipped(cmd.AggregateId, _shippingAddress));
        }

        internal void Deliver(DeliverOrder cmd)
        {
            if (_state != OrderState.Shipped) throw new InvalidOrderStateException(cmd.AggregateId, $"State should be {OrderState.Shipped} but is {_state}");

            RaiseEvent(new OrderDelivered(cmd.AggregateId, _shippingAddress));
        }

        private void Apply(OrderCancelled evt)
        {
            _state = OrderState.Cancelled;
        }

        private void Apply(OrderShipped evt)
        {
            _state = OrderState.Shipped;
        }

        private void Apply(OrderDelivered evt)
        {
            _state = OrderState.Delivered;
        }

        internal static IAggregate Create(CreateOrder cmd)
        {
            return new OrderAggregate(cmd);
        }
    }
}
