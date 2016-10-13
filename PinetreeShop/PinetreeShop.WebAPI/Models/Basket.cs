using PinetreeShop.Domain.Shared.Types;
using System;
using System.Collections.Generic;

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