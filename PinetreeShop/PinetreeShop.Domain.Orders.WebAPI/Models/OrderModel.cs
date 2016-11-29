using System;
using System.Collections.Generic;
using System.Linq;
using PinetreeShop.Domain.Orders.ReadModel.Entitites;

namespace PinetreeShop.Domain.Orders.WebAPI.Models
{
    public class OrderModel
    {
        public Guid Id { get; set; }
        public string State { get; set; }

        public IEnumerable<LineModel> Lines { get; set; }
        public AddressModel ShippingAddress { get; set; }
            

        internal static OrderModel FromEntity(Order entity)
        {
            return new OrderModel
            {
                Id = entity.Id,
                State = entity.State,
                Lines = (entity.Lines ?? new List<Line>()).Select(LineModel.FromEntity).ToList(),
                ShippingAddress = new AddressModel
                {
                    StreetAndNumber = entity.StreetAndNumber,
                    ZipAndCity = entity.ZipAndCity,
                    StateOrProvince = entity.StateOrProvince,
                    Country = entity.Country
                }
            };
        }

        internal static OrderModel FromAggregate(OrderAggregate agg)
        {
            return new OrderModel
            {
                Id = agg.AggregateId,
                State = agg.State.ToString(),
                Lines = agg.OrderLines.Select(LineModel.FromAggregate).ToList(),
                ShippingAddress = new AddressModel
                {
                    StreetAndNumber = agg.ShippingAddress.StreetAndNumber,
                    ZipAndCity = agg.ShippingAddress.ZipAndCity,
                    StateOrProvince = agg.ShippingAddress.StateOrProvince,
                    Country = agg.ShippingAddress.Country
                }
            };
        }
    }
}