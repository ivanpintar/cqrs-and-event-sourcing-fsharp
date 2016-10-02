using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.Domain.Types;
using System;

namespace PinetreeShop.Domain.Baskets.Events
{
    public class BasketCreated : EventBase
    {
    }

    public class ProductAdded : EventBase
    {
        public Guid ProductId { get; set; }
        public uint Quantity { get; set; }
    }

    public class ProductRemoved : EventBase
     {
        public Guid ProductId { get; set; }
        public uint Quantity { get; set; }
    }

    public class CheckedOut : EventBase
    {
        public Address Address { get; set; }
    }
}
