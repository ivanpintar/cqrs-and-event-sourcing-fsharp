using System;

namespace PinetreeShop.Domain.Baskets.WebAPI.Models
{
    public class AddItemModel
    {
        public Guid BasketId { get; set; }
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}