using System;
using PinetreeShop.Domain.Orders.ReadModel.Entitites;
using PinetreeShop.Domain.Shared.Types;

namespace PinetreeShop.Domain.Orders.WebAPI.Models
{
    public class LineModel
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        internal static LineModel FromEntity(Line arg)
        {
            return new LineModel
            {
                ProductId = arg.ProductId,
                ProductName = arg.ProductName,
                Price = arg.Price,
                Quantity = arg.Quantity
            };
        }

        internal static LineModel FromAggregate(OrderLine arg)
        {
            return new LineModel
            {
                ProductId = arg.ProductId,
                ProductName = arg.ProductName,
                Price = arg.Price,
                Quantity = arg.Quantity
            };
        }
    }
}