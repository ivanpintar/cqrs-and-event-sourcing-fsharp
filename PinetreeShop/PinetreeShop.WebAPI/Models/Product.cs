using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PinetreeShop.WebAPI.Models
{
    public class ProductModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public uint Quantity { get; set; }
        public uint AvailableQuantity { get; set; }
    }

    public class ChangeQuantityModel
    {
        public Guid ProductId { get; set; }
        public int Difference { get; set; }
    }

    public class ReleaseReservationModel
    {
        public Guid ProductId { get; set; }
        public uint Quantity { get; set; }
    }
}