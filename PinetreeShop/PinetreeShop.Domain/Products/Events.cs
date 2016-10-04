using PinetreeShop.CQRS.Infrastructure.CommandsAndEvents;
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
        public uint QuantityToReserve { get; private set; }

        public ProductReservationFailed(Guid productId, Guid basketId, uint quantityToReserve, string reason) : base(productId, reason)
        {
            BasketId = basketId;
            QuantityToReserve = quantityToReserve;
        }
    }

    public class ProductReservationReleased : EventBase
    {
        public uint QuantityToRelease { get; private set; }

        public ProductReservationReleased(Guid productId, uint quantityToRelease) : base(productId)
        {
            QuantityToRelease = quantityToRelease;
        }
    }
}
