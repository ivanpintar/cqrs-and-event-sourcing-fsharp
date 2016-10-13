using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.Domain.Shared.Types;
using System;
using System.Collections.Generic;

namespace PinetreeShop.Domain.Orders.Commands
{
    public class CreateOrder : CommandBase
    {
        public Guid BasketId { get; private set; }
        public IEnumerable<OrderLine> Lines { get; private set; }
        public Address ShippingAddress { get; private set; }

        public CreateOrder(Guid orderId, Guid basketId, IEnumerable<OrderLine> lines, Address shippingAddress) : base(orderId)
        {
            BasketId = basketId;
            Lines = lines;
            ShippingAddress = shippingAddress;
        }
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
