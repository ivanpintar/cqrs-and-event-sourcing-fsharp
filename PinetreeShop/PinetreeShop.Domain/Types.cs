using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinetreeShop.Domain.Types
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
        public int Quantity { get; set; }
    }
}
