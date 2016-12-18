using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PinetreeShop.CQRS.CommandAPI.Models
{
    public class CreateProductModel
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    public class ChangeQuantityModel
    {
        public Guid Id { get; set; }
        public int Difference { get; set; }
    }
}