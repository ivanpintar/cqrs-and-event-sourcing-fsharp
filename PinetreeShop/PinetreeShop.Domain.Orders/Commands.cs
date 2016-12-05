using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.Domain.Shared.Types;
using System;
using System.Collections.Generic;

namespace PinetreeShop.Domain.Orders.Commands
{
    public class CreateOrder : CommandBase
    {
        public Guid BasketId { get; private set; }
        public Address ShippingAddress { get; private set; }

        public CreateOrder(Guid orderId, Guid basketId, Address shippingAddress) : base(orderId)
        {
            BasketId = basketId;
            ShippingAddress = shippingAddress;
        }
    }

    public class AddOrderLine : CommandBase
    {
        public OrderLine OrderLine { get; private set; }

        public AddOrderLine(Guid orderId, OrderLine orderLine) : base(orderId)
        {
            OrderLine = orderLine;
        }        
    }

    public class PrepareOrderForShipping : CommandBase
    {
        public PrepareOrderForShipping(Guid orderId) : base(orderId) { }
    }

    public class CancelOrder : CommandBase
    {
        public CancelOrder(Guid orderId) : base(orderId)
        {
        }
    }

    public class ShipOrder : CommandBase
    {
        public ShipOrder(Guid orderId) : base(orderId)
        {
        }
    }

    public class DeliverOrder : CommandBase
    {
        public DeliverOrder(Guid orderId) : base(orderId)
        {
        }

    }
}
