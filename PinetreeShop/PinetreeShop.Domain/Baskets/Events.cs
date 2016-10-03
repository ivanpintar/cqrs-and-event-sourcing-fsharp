using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.CommandsAndEvents;
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

    public class AddProductFailed : EventBase
    {
        public Guid ProductId { get; private set; }
        public uint Quantity { get; private set; }
       
        public AddProductFailed(Guid aggregateId, Guid productId, uint quantity) : base(aggregateId)
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

    public class RemoveProductFailed : EventFailedBase
    {
        public Guid ProductId { get; private set; }
        public uint Quantity { get; private set; }

        public RemoveProductFailed(Guid aggregateId, Guid productId, uint quantity, string reason) : base(aggregateId, reason)
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

    public class CheckOutFailed : EventFailedBase
    {
        public CheckOutFailed(Guid aggregateId, string reason) : base(aggregateId, reason)
        {
        }
    }
}
