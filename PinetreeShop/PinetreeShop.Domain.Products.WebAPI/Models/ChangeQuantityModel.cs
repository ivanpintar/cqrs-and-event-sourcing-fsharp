using System;

namespace PinetreeShop.Domain.Products.WebAPI.Models
{
    public class ChangeQuantityModel
    {
        public int Difference { get; set; }
        public Guid Id { get; set; }
    }
}