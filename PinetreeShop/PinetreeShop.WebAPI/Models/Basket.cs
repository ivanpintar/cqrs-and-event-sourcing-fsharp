using PinetreeShop.Domain.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PinetreeShop.WebAPI.Models
{
    public class BasketModel
    {
        public Guid Id { get; set; }
        public IEnumerable<OrderLine> OrderLines { get; set; }
    }

    public class AddProductModel
    {
        public Guid ProductId { get; set; }
        public uint Quantity { get; set; }
    }

    public class RemoveProductModel
    {
        public Guid ProductId { get; set; }
        public uint Quantity { get; set; }
    }
}