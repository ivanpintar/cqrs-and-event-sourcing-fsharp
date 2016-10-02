using PinetreeShop.CQRS.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinetreeShop.Domain.Products.Events
{
    public class ProductCreated : EventBase
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    public class ProductQuantityChanged : EventBase
    {
        public int Difference { get; set; }
    }

    public class ProductReserved : EventBase
    {
        public Guid BasketId { get; set; }
        public uint Quantity { get; set; }
    }

    public class ProductReservationReleased : EventBase
    {
        public uint Quantity { get; set; }
    }
}
