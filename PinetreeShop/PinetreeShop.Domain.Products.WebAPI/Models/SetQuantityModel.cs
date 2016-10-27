using System;

namespace PinetreeShop.Domain.Products.WebAPI.Models
{
    public class SetQuantityModel
    {
        public int Quantity { get; internal set; }
        public Guid Id { get; internal set; }
    }
}