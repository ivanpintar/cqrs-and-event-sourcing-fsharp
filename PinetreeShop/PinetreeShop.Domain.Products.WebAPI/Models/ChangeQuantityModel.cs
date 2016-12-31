using System;

namespace PinetreeShop.Domain.Products.WebAPI.Models
{
    public class ChangeQuantityModel
    {
        public int Quantity { get; set; }
        public Guid Id { get; set; }
    }
}