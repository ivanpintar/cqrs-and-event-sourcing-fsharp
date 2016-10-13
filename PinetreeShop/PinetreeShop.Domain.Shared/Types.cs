using System;

namespace PinetreeShop.Domain.Shared.Types
{
    public class Address
    {
        public string StreetAndNumber { get; set; }
        public string ZipAndCity { get; set; }
        public string StateOrProvince { get; set; }
        public string Country { get; set; }
    }

    public class OrderLine
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public uint Quantity { get; set; }
    }
}
