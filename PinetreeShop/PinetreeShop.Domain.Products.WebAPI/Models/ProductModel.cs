using System;

namespace PinetreeShop.Domain.Products.WebAPI.Models
{
    public class ProductModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int Reserved { get; set; }

        public static ProductModel FromEntity(object productEntity)
        {
            throw new NotImplementedException();
        }        
    }
}