using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.Domain.Shared.Types;
using System;

namespace PinetreeShop.Domain.Baskets.Events
{
    public class BasketCreated : EventBase
    {
        public BasketCreated(Guid basketId) : base(basketId)
        {
        }
    }

    public class BasketItemAdded : EventBase
    {
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }
        public string ProductName { get; private set; }
        public decimal Price { get; private set; }

        public BasketItemAdded(Guid basketId, Guid productId, string productName, decimal price, int quantity) : base(basketId)
        {
            ProductId = productId;
            ProductName = productName;
            Quantity = quantity;
            Price = price;
        }
    }

    public class BasketItemRemoved : EventBase
    {
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }

        public BasketItemRemoved(Guid basketId, Guid productId, int quantity) : base(basketId)
        {
            ProductId = productId;
            Quantity = quantity;
        }
    }
    
    public class BasketCancelled : EventBase
    {
        public BasketCancelled(Guid basketId) : base(basketId)
        {
        }
    }

    public class BasketCheckedOut : EventBase
    {
        public Address ShippingAddress { get; private set; }

        public BasketCheckedOut(Guid basketId, Address shippingAddress) : base(basketId)
        {
            ShippingAddress = shippingAddress;
        }
    }
}
