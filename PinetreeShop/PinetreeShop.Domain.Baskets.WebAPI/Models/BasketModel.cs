using System;
using System.Linq;
using System.Collections.Generic;
using static PinetreeCQRS.Infrastructure.Types;

namespace PinetreeShop.Domain.Baskets.WebAPI.Models
{
    public class Item
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        internal static Item FromAggregate(BasketItem value)
        {
            return new Item
            {
                ProductId = value.ProductId.Item,
                Price = value.Price,
                ProductName = value.ProductName,
                Quantity = value.Quantity
            };
        }
    }

    public class BasketModel
    {
        public Guid Id { get; set; }
        public List<Item> Items { get; set; }
        public string State { get; set; }

        public BasketModel()
        {
            Items = new List<Item>();
        }

        internal static BasketModel FromAggregate(Guid basketId, BasketAggregate.State basket)
        {
            return new BasketModel {
                Id = basketId,
                State = basket.BasketState.ToString(),
                Items = basket.Items.Select(i => Item.FromAggregate(i.Value)).ToList()                
            };

        }
    }
}