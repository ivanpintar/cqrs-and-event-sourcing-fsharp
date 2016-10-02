using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.Domain.Types;
using System;

namespace PinetreeShop.Domain.Baskets.Events
{
    public class BasketCreated : EventBase
    {
        public BasketCreated(Guid aggregateId) : base(aggregateId)
        {
        }
    }

    public class ProductAdded : EventBase
    {
        public Guid ProductId { get; private set; }
        public uint Quantity { get; private set; }

        public ProductAdded(Guid aggregateId, Guid productId, uint quantity) : base(aggregateId)
        {
            ProductId = productId;
            Quantity = quantity;
        }
    }

    public class ProductRemoved : EventBase
    {
        public Guid ProductId { get; private set; }
        public uint Quantity { get; private set; }

        public ProductRemoved(Guid aggregateId, Guid productId, uint quantity) : base(aggregateId)
        {
            ProductId = productId;
            Quantity = quantity;
        }
    }

    public class Cancelled : EventBase
    {
        public Cancelled(Guid aggregateId) : base(aggregateId)
        {
        }
    }

    public class CheckedOut : EventBase
    {
        public Address Address { get; private set; }

        public CheckedOut(Guid aggregateId, Address address) : base(aggregateId)
        {
            Address = address;
        }
    }
}
