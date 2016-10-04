using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.CommandsAndEvents;
using System;

namespace PinetreeShop.Domain.Products.Commands
{
    public class CreateProduct : CommandBase
    {
        public string Name { get; private set; }
        public decimal Price { get; private set; }

        public CreateProduct(Guid productId, string name, decimal price) : base(productId)
        {
            Name = name;
            Price = price;
        }
    }

    public class ChangeProductQuantity : CommandBase
    {
        public int Difference { get; private set; }

        public ChangeProductQuantity(Guid productId, int difference) : base(productId)
        {
            Difference = difference;
        }
    }

    public class ReserveProduct : CommandBase
    {
        public uint QuantityToReserve { get; private set; }
        public Guid BasketId { get; private set; }

        public ReserveProduct(Guid productId, Guid basketId, uint quantityToReserve) : base(productId)
        {
            QuantityToReserve = quantityToReserve;
            BasketId = basketId;
        }
    }

    public class ReleaseProductReservation : CommandBase
    {
        public uint QuantityToRelease { get; private set; }

        public ReleaseProductReservation(Guid productId, uint quantityToRelease) : base(productId)
        {
            QuantityToRelease = quantityToRelease;
        }
    }
}
