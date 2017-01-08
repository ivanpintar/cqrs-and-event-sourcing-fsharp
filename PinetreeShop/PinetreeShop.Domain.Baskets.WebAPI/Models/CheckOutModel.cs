using System;

namespace PinetreeShop.Domain.Baskets.WebAPI.Models
{
    public class CheckOutModel
    {
        public Guid BasketId { get; set; }
        public string ShippingAddress { get; set; }
    }
}