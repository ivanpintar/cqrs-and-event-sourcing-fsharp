using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PinetreeShop.Domain.Shared.Types;

namespace PinetreeShop.Domain.Baskets.WebAPI.Models
{
    public class BasketModel
    {
        public Guid Id { get; private set; }
        public List<OrderLine> OrderLines { get; private set; }
        public BasketAggregate.BasketState State { get; private set; }

        public static BasketModel FromAggregate(BasketAggregate basket)
        {
            return new BasketModel
            {
                Id = basket.AggregateId,
                OrderLines = basket.OrderLines.ToList(),
                State = basket.State
            };
        }
    }
}