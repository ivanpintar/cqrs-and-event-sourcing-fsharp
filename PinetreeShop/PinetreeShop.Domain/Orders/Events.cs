using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.CommandsAndEvents;
using PinetreeShop.Domain.Types;
using System;
using System.Collections.Generic;

namespace PinetreeShop.Domain.Orders.Events
{
    public class OrderCreated : EventBase
    {
        public Guid BasketId { get; private set; }
        public IEnumerable<Types.OrderLine> Lines { get; private set; }

        public OrderCreated(Guid aggregateId, Guid basketId, IEnumerable<OrderLine> lines) : base(aggregateId)
        {
            BasketId = basketId;
            Lines = lines;
        }
    }

    public class OrderCancelled : EventBase
    {
        public OrderCancelled(Guid aggregateId) : base(aggregateId)
        {
        }
    }

    public class OrderShipped : EventBase
    {
        public Address Address { get; private set; }

        public OrderShipped(Guid aggregateId, Address address) : base(aggregateId)
        {
            Address = address;
        }
    }

    public class OrderDelivered : EventBase
    {
        public Address Address { get; private set; }

        public OrderDelivered(Guid aggregateId, Address address) : base(aggregateId)
        {
            Address = address;
        }
    }
}
