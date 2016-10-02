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
                Id = basket.Id,
                OrderLines = basket.OrderLines.ToList()
            };
        }

        public static ProductModel ToModel(this Product product)
        {
            return new ProductModel
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Quantity = product.Quantity,
                AvailableQuantity = product.AvailableQuantity
            };
        }
    }
}