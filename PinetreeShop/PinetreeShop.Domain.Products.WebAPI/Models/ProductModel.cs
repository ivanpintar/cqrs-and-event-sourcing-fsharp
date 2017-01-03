using System;
using static PinetreeShop.Domain.Products.ReadModel;

namespace PinetreeShop.Domain.Products.WebAPI.Models
{
    public class ProductModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int Reserved { get; set; }

        internal static ProductModel FromDTO(ProductDTO p)
        {
            return new ProductModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Quantity = p.Quantity,
                Reserved = p.Reserved
            };
        }
    }
}