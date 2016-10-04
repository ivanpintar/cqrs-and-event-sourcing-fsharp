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
        public string ProductName { get; private set; }
        public decimal Price { get; private set; }

        public ProductAdded(Guid basketId, Guid productId, string productName, decimal price, uint quantity) : base(basketId)
        {
            ProductId = productId;
            ProductName = productName;
            Quantity = quantity;
            Price = price;
        }
    }

    public class AddProductReverted : EventFailedBase
    {
        public Guid ProductId { get; private set; }
        public uint Quantity { get; private set; }
       
        public AddProductReverted(Guid basketId, Guid productId, uint quantity, string reason) : base(basketId, reason)
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
    
    public class Cancelled : EventBase
    {
        public Cancelled(Guid basketId) : base(basketId)
        {
        }
    }

    public class CheckedOut : EventBase
    {
        public Address ShippintAddress { get; private set; }

        public CheckedOut(Guid basketId, Address shippingAddress) : base(basketId)
        {
            ShippintAddress = shippingAddress;
        }
    }

    public class CheckOutReverted : EventFailedBase
    {
        public CheckOutReverted(Guid basketId, string reason) : base(basketId, reason)
        {
        }
    }
}
