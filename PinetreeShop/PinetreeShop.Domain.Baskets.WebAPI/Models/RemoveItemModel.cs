using System;

namespace PinetreeShop.Domain.Baskets.WebAPI.Models
{
    public class RemoveItemModel
    {
        public Guid BasketId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}