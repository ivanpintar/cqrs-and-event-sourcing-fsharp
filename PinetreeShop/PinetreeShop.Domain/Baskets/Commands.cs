using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.CommandsAndEvents;
using PinetreeShop.Domain.Types;
using System;

namespace PinetreeShop.Domain.Baskets.Commands
{
    public class CreateBasket : CommandBase
    {
        public CreateBasket(Guid basketId) : base(basketId)
        {

        }
    }

    public class AddProduct : CommandBase
    {
        public Guid ProductId { get; set; }
        public uint Quantity { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }

        public AddProduct(Guid basketId, Guid productId, string productName, decimal price, uint quantity) : base(basketId)
        {
            ProductId = productId;
            Quantity = quantity;
            ProductName = productName;
            Price = price;
        }
    }

    public class RevertAddProduct : RevertCommandBase
    {
        public Guid ProductId { get; set; }
        public uint Quantity { get; set; }

        public RevertAddProduct(Guid basketId, Guid productId, uint quantity, string reason) : base(basketId, reason)
        {
            ProductId = productId;
            Quantity = quantity;
        }
    }

    public class RemoveProduct : CommandBase
    {
        public Guid ProductId { get; set; }
        public uint Quantity { get; set; }

        public RemoveProduct(Guid basketId, Guid productId, uint quantity) : base(basketId)
        {
            ProductId = productId;
            Quantity = quantity;
        }
    }

    public class Cancel : CommandBase
    {
        public Cancel(Guid basketId) : base(basketId)
        {

        }
    }

    public class CheckOut : CommandBase
    {
        public Address ShippingAddress { get; set; }

        public CheckOut(Guid basketId, Address shippingAddress) : base(basketId)
        {
            ShippingAddress = shippingAddress;
        }
    }

    public class RevertCheckOut : RevertCommandBase
    {
        public RevertCheckOut(Guid basketId, string reason) : base(basketId, reason) { }
    }

}
