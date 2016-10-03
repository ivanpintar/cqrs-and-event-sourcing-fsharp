using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.CommandsAndEvents;
using System;

namespace PinetreeShop.Domain.Products.Commands
{
    public class CreateProduct : CommandBase
    {
        public string Name { get; private set; }
        public decimal Price { get; private set; }

        public CreateProduct(Guid aggregateId, string name, decimal price) : base(aggregateId)
        {
            Name = name;
            Price = price;
        }
    }

    public class ChangeProductQuantity : CommandBase
    {
        public int Difference { get; private set; }

        public ChangeProductQuantity(Guid aggregateId, int difference) : base(aggregateId)
        {
            Difference = difference;
        }
    }

    public class ReserveProduct : CommandBase
    {
        public uint QuantityToReserve { get; private set; }
        public Guid BasketId { get; private set; }

        public ReserveProduct(Guid aggregateId, Guid basketId, uint quantityToReserve) : base(aggregateId)
        {
            QuantityToReserve = quantityToReserve;
            BasketId = basketId;
        }
    }

    public class ReleaseProductReservation : CommandBase
    {
        public uint QuantityToRelease { get; private set; }

        public ReleaseProductReservation(Guid aggregateId, uint quantityToRelease) : base(aggregateId)
        {
            QuantityToRelease = quantityToRelease;
        }
    }
}
