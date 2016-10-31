using PinetreeShop.Domain.Products.ReadModel.Entities;
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

        public static ProductModel FromEntity(Product productEntity)
        {
            return new ProductModel
            {
                Id = productEntity.Id,
                Name = productEntity.Name,
                Price = productEntity.Price,
                Quantity = productEntity.Quantity,
                Reserved = productEntity.Reserved
            };
        }

        public static ProductModel FromAggregate(ProductAggregate productAggregate)
        {
            return new ProductModel
            {
                Id = productAggregate.AggregateId,
                Name = productAggregate.Name,
                Price = productAggregate.Price,
                Quantity = productAggregate.Quantity,
                Reserved = productAggregate.Reserved
            };
        }
    }
}