using PinetreeShop.Domain.Shared.Types;
using System;

namespace PinetreeShop.Domain.Baskets.WebAPI.Models
{
    public class CheckOutModel
    {
        public Guid BasketId { get; set; }
        public Address ShippingAddress { get; set; }
    }
}