using System;

namespace PinetreeShop.Domain.Orders.WebAPI.Models
{
    public class GuidModel
    {
        public Guid OrderId { get; set; }
        public Guid ProcessId { get; set; }
    }
}