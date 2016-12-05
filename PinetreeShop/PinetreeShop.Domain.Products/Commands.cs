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

    public class RemoveProductFromStock : CommandBase
    {
        public int Quantity { get; private set; }

        public RemoveProductFromStock(Guid productId, int quantity) : base(productId)
        {
            Quantity = quantity;
        }
    }

    public class AddProductToStock : CommandBase
    {
        public int Quantity { get; private set; }

        public AddProductToStock(Guid productId, int quantity) : base(productId)
        {
            Quantity = quantity;
        }
    }

    public class SetProductQuantity : CommandBase
    {
        public int Quantity { get; private set; }

        public SetProductQuantity(Guid productId, int quantity) : base(productId)
        {
             Quantity = quantity;
        }
    }

    public class ReserveProduct : CommandBase
    {
        public int Quantity { get; private set; }

        public ReserveProduct(Guid productId, int quantity) : base(productId)
        {
            Quantity = quantity;
        }
    }

    public class CancelProductReservation : CommandBase
    {
        public int Quantity { get; private set; }

        public CancelProductReservation(Guid productId, int quantity) : base(productId)
        {
            Quantity = quantity;
        }
    }

    public class PurchaseReservedProduct : CommandBase
    {
        public int Quantity { get; private set; }

        public PurchaseReservedProduct(Guid productId, int quantity) : base(productId)
        {
            Quantity = quantity;
        }
    }
}
