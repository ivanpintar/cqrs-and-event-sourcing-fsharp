using PinetreeCQRS.Infrastructure;
using PinetreeShop.Domain.Orders.Commands;
using PinetreeShop.Domain.Orders.Events;
using PinetreeShop.Domain.Shared.Exceptions;
using PinetreeShop.Domain.Shared.Types;
using PinetreeShop.Domain.Tests.Order.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PinetreeShop.Domain.Orders
{
    public class OrderAggregate : AggregateBase
    {
        public enum OrderState { Pending, ReadyForShipping, Shipped, Cancelled, Delivered };
        public OrderState State { get; set; }
        public IList<OrderLine> OrderLines { get; set; }
        public Guid BasketId { get; set; }
        public Guid ProcessId { get; set; }
        public Address ShippingAddress { get; set; }

        #region Constructors 

        public OrderAggregate()
        {
            RegisterEventHandler<OrderCreated>(Apply);
            RegisterEventHandler<OrderLineAdded>(Apply);
            RegisterEventHandler<OrderReadyForShipping>(Apply);
            RegisterEventHandler<OrderCancelled>(Apply);
            RegisterEventHandler<OrderShipped>(Apply);
            RegisterEventHandler<OrderDelivered>(Apply);
        }

        public OrderAggregate(CreateOrder cmd) : base()
        {
            var orderId = cmd.AggregateId;
            var basketId = cmd.BasketId;
            var shippingAddress = cmd.ShippingAddress;

            if (shippingAddress == null) throw new ParameterNullException(orderId, "shippingAddress");

            RaiseEvent(new OrderCreated(orderId, basketId, shippingAddress));
        }

#endregion

        #region Event handlers

        private void Apply(OrderReadyForShipping obj)
        {
            State = OrderState.ReadyForShipping;
        }

        private void Apply(OrderLineAdded obj)
        {
            OrderLines.Add(obj.OrderLine);
        }

        private void Apply(OrderCreated evt)
        {
            State = OrderState.Pending;
            OrderLines = new List<OrderLine>();

            AggregateId = evt.AggregateId;
            BasketId = evt.BasketId;
            ProcessId = evt.Metadata.ProcessId;
            ShippingAddress = evt.ShippingAddress;
        }

        private void Apply(OrderCancelled evt)
        {
            State = OrderState.Cancelled;
        }

        private void Apply(OrderShipped evt)
        {
            State = OrderState.Shipped;
        }

        private void Apply(OrderDelivered evt)
        {
            State = OrderState.Delivered;
        }

        #endregion

        #region Command handlers

        internal void PrepareForShipping(PrepareOrderForShipping cmd)
        {
            if (State != OrderState.Pending)
                throw new InvalidOrderStateException(cmd.AggregateId, $"State should be {OrderState.Pending} but is {State}");

            if (State == OrderState.ReadyForShipping)
                return;

            if(!OrderLines.Any())
                throw new InvalidOrderStateException(cmd.AggregateId, $"Order has no order lines");

            RaiseEvent(new OrderReadyForShipping(cmd.AggregateId));
        }

        internal void AddOrderLine(AddOrderLine cmd)
        {
            if(State != OrderState.Pending)
                throw new InvalidOrderStateException(cmd.AggregateId, $"State should be {OrderState.Pending} but is {State}");

            RaiseEvent(new OrderLineAdded(cmd.AggregateId, cmd.OrderLine));
        }

        internal void Cancel(CancelOrder cmd)
        {
            switch (State)
            {
                case OrderState.Shipped:
                case OrderState.Delivered:
                    throw new InvalidOrderStateException(cmd.AggregateId, $"State should be {OrderState.Pending} or {OrderState.ReadyForShipping} but is {State}");
                default:
                    RaiseEvent(new OrderCancelled(cmd.AggregateId));
                    break;
            }
        }

        internal void Ship(ShipOrder cmd)
        {
            if (State != OrderState.ReadyForShipping)
                throw new InvalidOrderStateException(cmd.AggregateId, $"State should be {OrderState.ReadyForShipping} but is {State}");

            RaiseEvent(new OrderShipped(cmd.AggregateId));
        }

        internal void Deliver(DeliverOrder cmd)
        {
            if (State != OrderState.Shipped) 
                throw new InvalidOrderStateException(cmd.AggregateId, $"State should be {OrderState.Shipped} but is {State}");

            RaiseEvent(new OrderDelivered(cmd.AggregateId));
        }

        internal static OrderAggregate Create(CreateOrder cmd)
        {
            return new OrderAggregate(cmd);
        }

        #endregion
    }
}
