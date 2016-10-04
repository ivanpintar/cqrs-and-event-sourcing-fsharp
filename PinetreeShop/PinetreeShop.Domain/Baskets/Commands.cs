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

        public AddProduct(Guid basketId, Guid productId, uint quantity) : base(basketId)
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

    public class Checkout : CommandBase
    {
        public Address Address { get; set; }

        public Checkout(Guid basketId, Address address) : base(basketId)
        {
            Address = address;
        }
    }

}
