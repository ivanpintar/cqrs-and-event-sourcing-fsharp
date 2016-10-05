using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.Commands;
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
        public uint Quantity { get; private set; }
        public Guid BasketId { get; private set; }

        public ReserveProduct(Guid productId, Guid basketId, uint quantity) : base(productId)
        {
            Quantity = quantity;
            BasketId = basketId;
        }
    }

    public class CancelProductReservation : CommandBase
    {
        public uint Quantity { get; private set; }

        public CancelProductReservation(Guid productId, uint quantity) : base(productId)
        {
            Quantity = quantity;
        }
    }
}
