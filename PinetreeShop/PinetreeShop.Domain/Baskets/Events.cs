using PinetreeShop.CQRS.Infrastructure.CommandsAndEvents;
using PinetreeShop.Domain.Types;
using System;

namespace PinetreeShop.Domain.Baskets.Events
{
    public class BasketCreated : EventBase
    {
        public BasketCreated(Guid basketId) : base(basketId)
        {
        }
    }

    public class ProductAdded : EventBase
    {
        public Guid ProductId { get; private set; }
        public uint Quantity { get; private set; }

        public ProductAdded(Guid basketId, Guid productId, uint quantity) : base(basketId)
        {
            ProductId = productId;
            Quantity = quantity;
        }
    }

    public class AddProductFailed : EventFailedBase
    {
        public Guid ProductId { get; private set; }
        public uint Quantity { get; private set; }
       
        public AddProductFailed(Guid basketId, Guid productId, uint quantity, string reason) : base(basketId, reason)
        {
            ProductId = productId;
            Quantity = quantity;
        }
    }

    public class ProductRemoved : EventBase
    {
        public Guid ProductId { get; private set; }
        public uint Quantity { get; private set; }

        public ProductRemoved(Guid basketId, Guid productId, uint quantity) : base(basketId)
        {
            ProductId = productId;
            Quantity = quantity;
        }
    }

    public class RemoveProductFailed : EventFailedBase
    {
        public Guid ProductId { get; private set; }
        public uint Quantity { get; private set; }

        public RemoveProductFailed(Guid basketId, Guid productId, uint quantity, string reason) : base(basketId, reason)
        {
            ProductId = productId;
            Quantity = quantity;
        }
    }

    public class Cancelled : EventBase
    {
        public Cancelled(Guid basketId) : base(basketId)
        {
        }
    }

    public class CheckedOut : EventBase
    {
        public Address Address { get; private set; }

        public CheckedOut(Guid basketId, Address address) : base(basketId)
        {
            Address = address;
        }
    }

    public class CheckOutFailed : EventFailedBase
    {
        public CheckOutFailed(Guid basketId, string reason) : base(basketId, reason)
        {
        }
    }
}
