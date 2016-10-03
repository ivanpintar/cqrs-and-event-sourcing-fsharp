using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.CommandsAndEvents;
using PinetreeShop.Domain.Types;
using System;

namespace PinetreeShop.Domain.Baskets.Commands
{
    public class CreateBasket : CommandBase
    {
        public CreateBasket(Guid aggregateId) : base(aggregateId)
        {

        }
    }

    public class AddProduct : CommandBase
    {
        public Guid ProductId { get; set; }
        public uint Quantity { get; set; }

        public AddProduct(Guid aggregateId, Guid productId, uint quantity) : base(aggregateId)
        {
            ProductId = productId;
            Quantity = quantity;
        }
    }

    public class RemoveProduct : CommandBase
    {
        public Guid ProductId { get; set; }
        public uint Quantity { get; set; }

        public RemoveProduct(Guid aggregateId, Guid productId, uint quantity) : base(aggregateId)
        {
            ProductId = productId;
            Quantity = quantity;
        }
    }

    public class Cancel : CommandBase
    {
        public Cancel(Guid aggregateId) : base(aggregateId)
        {

        }
    }

    public class Checkout : CommandBase
    {
        public Address Address { get; set; }

        public Checkout(Guid aggregateId, Address address) : base(aggregateId)
        {
            Address = address;
        }
    }

}
