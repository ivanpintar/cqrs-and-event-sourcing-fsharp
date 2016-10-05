using PinetreeShop.Domain.Baskets;
using PinetreeShop.Domain.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PinetreeShop.WebAPI.Models
{
    public static class Extensions
    {
        public static BasketModel ToModel(this Basket basket)
        {
            return new BasketModel
            {
                Id = basket.AggregateId,
                OrderLines = basket.OrderLines.ToList()
            };
        }

        public static ProductModel ToModel(this Product product)
        {
            return new ProductModel
            {
                Id = product.AggregateId,
                Name = product.Name,
                Price = product.Price,
                Quantity = product.Quantity,
                AvailableQuantity = product.AvailableQuantity
            };
        }
    }
}