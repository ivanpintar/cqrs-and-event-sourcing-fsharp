using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.Domain.Types;
using System;
using System.Collections.Generic;

namespace PinetreeShop.Domain.Orders.Events
{
    public class OrderCreated : EventBase
    {
        public Guid BasketId { get; set; }
        public IEnumerable<Types.OrderLine> Lines { get; set; }
    }

    public class OrderCancelled : EventBase
    {
    }

    public class OrderShipped : EventBase
    {
        public Address Address { get; set; }
    }

    public class OrderDelivered : EventBase
    {
        public Address Address { get; set; }
    }
}
