using PinetreeShop.CQRS.Infrastructure.Events;
using System;

namespace PinetreeShop.Domain.Products.Events
{
    public class ProductCreated : EventBase
    {
        public string Name { get; private set; }
        public decimal Price { get; private set; }

        public ProductCreated(Guid productId, string name, decimal price) : base(productId)
        {
            Name = name;
            Price = price;
        }
    }

    public class ProductQuantityChanged : EventBase
    {
        public int Difference { get; private set; }

        public ProductQuantityChanged(Guid productId, int difference) : base(productId)
        {
            Difference = difference;
        }
    }

    public class ProductReserved : EventBase
    {
        public Guid BasketId { get; private set; }
        public uint QuantityToReserve { get; private set; }

        public ProductReserved(Guid productId, Guid basketId, uint quantityToReserve) : base(productId)
        {
            BasketId = basketId;
            QuantityToReserve = quantityToReserve;
        }
    }

    public class ProductReservationFailed : EventFailedBase
    {
        public static string NotAvailable = "NotAvailable";

        public Guid BasketId { get; private set; }
        public uint Quantity { get; private set; }

        public ProductReservationFailed(Guid productId, Guid basketId, uint quantity, string reason) : base(productId, reason)
        {
            BasketId = basketId;
            Quantity = quantity;
        }
    }

    public class ProductReservationCancelled : EventBase
    {
        public uint Quantity { get; private set; }

        public ProductReservationCancelled(Guid productId, uint quantity) : base(productId)
        {
            Quantity = quantity;
        }
    }
}
