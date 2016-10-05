using PinetreeShop.CQRS.Infrastructure.Commands;
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

    public class TryAddItemToBasket : CommandBase
    {
        public Guid ProductId { get; set; }
        public uint Quantity { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }

        public TryAddItemToBasket(Guid basketId, Guid productId, string productName, decimal price, uint quantity) : base(basketId)
        {
            ProductId = productId;
            Quantity = quantity;
            ProductName = productName;
            Price = price;
        }        
    }

    public class ConfirmAddItemToBasket : CommandBase
    {
        public Guid ProductId { get; set; }
        public uint Quantity { get; set; }

        public ConfirmAddItemToBasket(Guid basketId, Guid productId, uint quantity) : base(basketId)
        {
            ProductId = productId;
            Quantity = quantity;
        }
    }

    public class RevertAddItemToBasket : RevertCommandBase
    {
        public Guid ProductId { get; set; }
        public uint Quantity { get; set; }

        public RevertAddItemToBasket(Guid basketId, Guid productId, uint quantity, string reason) : base(basketId, reason)
        {
            ProductId = productId;
            Quantity = quantity;
        }
    }

    public class RemoveItemFromBasket : CommandBase
    {
        public Guid ProductId { get; set; }
        public uint Quantity { get; set; }

        public RemoveItemFromBasket(Guid basketId, Guid productId, uint quantity) : base(basketId)
        {
            ProductId = productId;
            Quantity = quantity;
        }
    }

    public class CancelBasket : CommandBase
    {
        public CancelBasket(Guid basketId) : base(basketId)
        {

        }
    }

    public class CheckOutBasket : CommandBase
    {
        public Address ShippingAddress { get; set; }

        public CheckOutBasket(Guid basketId, Address shippingAddress) : base(basketId)
        {
            ShippingAddress = shippingAddress;
        }
    }

}
