using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.Domain.Shared.Types;
using System;
using System.Collections.Generic;

namespace PinetreeShop.Domain.Orders.Events
{
    public class OrderCreated : EventBase
    {
        public Guid BasketId { get; private set; }
        public Address ShippingAddress { get; private set; }

        public OrderCreated(Guid orderId, Guid basketId, Address shippingAddress) : base(orderId)
        {
            BasketId = basketId;
            ShippingAddress = shippingAddress;
        }
    }

    public class OrderLineAdded : EventBase
    {
        public OrderLine OrderLine { get; private set; }

        public OrderLineAdded(Guid orderId, OrderLine orderLine) : base(orderId)
        {
            OrderLine = orderLine;
        }
    }

    public class CreateOrderFailed : EventFailedBase
    {
        public Guid BasketId { get; private set; }

        public CreateOrderFailed(Guid orderId, Guid basketId, string reason) : base(orderId, reason)
        {
            BasketId = basketId;
        }
    }

    public class OrderReadyForShipping : EventBase
    {
        public OrderReadyForShipping(Guid orderId) : base(orderId)
        {
        }
    }

    public class OrderCancelled : EventBase
    {
        public OrderCancelled(Guid orderId) : base(orderId)
        {
        }
    }
    
    public class OrderShipped : EventBase
    {
        public OrderShipped(Guid orderId) : base(orderId)
        {
        }
    }

    public class OrderDelivered : EventBase
    {
        public OrderDelivered(Guid orderId) : base(orderId)
        {
        }
    }
}
