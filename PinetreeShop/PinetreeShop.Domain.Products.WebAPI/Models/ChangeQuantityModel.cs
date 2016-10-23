using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PinetreeShop.Domain.Products.WebAPI.Models
{
    public class ChangeQuantityModel
    {
        public int Difference { get; internal set; }
        public Guid Id { get; internal set; }
    }
}