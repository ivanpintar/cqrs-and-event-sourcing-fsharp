using System;

namespace PinetreeShop.Domain.Products.WebAPI.Models
{
    public class SetQuantityModel
    {
        public int Quantity { get; set; }
        public Guid Id { get; set; }
    }
}