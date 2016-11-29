using System;
using System.Collections.Generic;

namespace PinetreeShop.Domain.Orders.ReadModel.Entitites
{
    public class Order
    {
        public Guid Id { get; set; }
        public string State { get; set; }

        public ICollection<Line> Lines { get; set; }

        public string StreetAndNumber { get; set; }
        public string ZipAndCity { get; set; }
        public string StateOrProvince { get; set; }
        public string Country { get; set; }
        public int LastEventNumber { get; set; }

        public Order()
        {
            Lines = new List<Line>();
        }
    }
}
