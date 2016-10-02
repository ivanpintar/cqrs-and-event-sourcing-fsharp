using PinetreeShop.CQRS.Infrastructure;
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
        public uint Quantity { get; private set; }

        public ReserveProduct(Guid aggregateId, uint quantity) : base(aggregateId)
        {
            Quantity = quantity;
        }
    }

    public class ReleaseProductReservation : CommandBase
    {
        public uint Quantity { get; private set; }

        public ReleaseProductReservation(Guid aggregateId, uint quantity) : base(aggregateId)
        {
            Quantity = quantity;
        }
    }
}
