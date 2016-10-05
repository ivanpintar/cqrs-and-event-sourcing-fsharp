using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.Domain.Types;
using System;
using System.Collections.Generic;

namespace PinetreeShop.Domain.Orders.Events
{
    public class OrderCreated : EventBase
    {
        public Guid BasketId { get; private set; }
        public IEnumerable<Types.OrderLine> Lines { get; private set; }
        public Address ShippingAddress { get; private set; }

        public OrderCreated(Guid aggregateId, Guid basketId, IEnumerable<OrderLine> lines, Address shippingAddress) : base(aggregateId)
        {
            BasketId = basketId;
            Lines = lines;
            ShippingAddress = shippingAddress;
        }
    }

    public class CreateOrderFailed : EventFailedBase
    {
        public Guid BasketId { get; private set; }

        public CreateOrderFailed(Guid aggregateId, Guid basketId, string reason) : base(aggregateId, reason)
        {
            BasketId = basketId;
        }
    }


    public class OrderCancelled : EventBase
    {
        public OrderCancelled(Guid orderId) : base(orderId)
        {
        }
    }

    public class CancelOrderFailed : EventFailedBase
    {
        public static string OrderShipped = "OrderShipped";
        public static string OrderDelivered = "OrderDelivered";

        public CancelOrderFailed(Guid orderId, string reason) : base(orderId, reason)
        {
        }
    }

    public class OrderShipped : EventBase
    {
        public Address ShippingAddress { get; private set; }

        public OrderShipped(Guid orderId, Address shippingAddress) : base(orderId)
        {
            ShippingAddress = shippingAddress;
        }
    }

    public class OrderDelivered : EventBase
    {
        public Address ShippingAddress { get; private set; }

        public OrderDelivered(Guid orderId, Address shippingAddress) : base(orderId)
        {
            ShippingAddress = shippingAddress;
        }
    }
}
