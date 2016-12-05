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
        public int QuantityToReserve { get; private set; }

        public ProductReserved(Guid productId, int quantityToReserve) : base(productId)
        {
            QuantityToReserve = quantityToReserve;
        }
    }

    public class ProductReservationFailed : EventFailedBase
    {
        public static string NotAvailable = "NotAvailable";

        public int Quantity { get; private set; }

        public ProductReservationFailed(Guid productId, int quantity, string reason) : base(productId, reason)
        {
            Quantity = quantity;
        }
    }

    public class ProductReservationCancelled : EventBase
    {
        public int Quantity { get; private set; }

        public ProductReservationCancelled(Guid productId, int quantity) : base(productId)
        {
            Quantity = quantity;
        }
    }

    public class ReservedProductPurchased : EventBase
    {
        public int Quantity { get; private set; }

        public ReservedProductPurchased(Guid productId, int quantity) : base(productId)
        {
            Quantity = quantity;
        }
    }
}
