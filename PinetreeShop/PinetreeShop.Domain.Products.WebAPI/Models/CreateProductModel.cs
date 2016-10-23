using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PinetreeShop.Domain.Products.WebAPI.Models
{
    public class CreateProductModel
    {
        public string Name { get; internal set; }
        public decimal Price { get; internal set; }
    }
}