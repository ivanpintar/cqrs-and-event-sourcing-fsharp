using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.CommandsAndEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinetreeShop.Domain.Products.Events
{
    public class ProductCreated : EventBase
    {
        public string Name { get; private set; }
        public decimal Price { get; private set; }

        public ProductCreated(Guid aggregateId, string name, decimal price) : base(aggregateId)
        {
            Name = name;
            Price = price;
        }
    }

    public class ProductQuantityChanged : EventBase
    {
        public int Difference { get; private set; }

        public ProductQuantityChanged(Guid aggregateId, int difference) : base(aggregateId)
        {
            Difference = difference;
        }
    }

    public class ProductReserved : EventBase
    {
        public Guid BasketId { get; private set; }
        public uint QuantityToReserve { get; private set; }

        public ProductReserved(Guid aggregateId, Guid basketId, uint quantityToReserve) : base(aggregateId)
        {
            BasketId = basketId;
            QuantityToReserve = quantityToReserve;
        }
    }

    public class ProductReservationFailed : EventFailedBase
    {
        public static string NotAvailable = "not available";

        public Guid BasketId { get; private set; }
        public uint QuantityToReserve { get; private set; }

        public ProductReservationFailed(Guid aggregateId, Guid basketId, uint quantityToReserve, string reason) : base(aggregateId, reason)
        {
            BasketId = basketId;
            QuantityToReserve = quantityToReserve;
        }
    }

    public class ProductReservationReleased : EventBase
    {
        public uint QuantityToRelease { get; private set; }

        public ProductReservationReleased(Guid aggregateId, uint quantityToRelease) : base(aggregateId)
        {
            QuantityToRelease = quantityToRelease;
        }
    }
}
