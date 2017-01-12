using System;
using System.Collections.Generic;
using System.Linq;

namespace PinetreeShop.Domain.Orders.WebAPI.Models
{
    public class OrderModel
    {
        public Guid Id { get; set; }
        public string State { get; set; }
        public Guid ProcessId { get; set; }

        public IEnumerable<LineModel> Lines { get; set; }
        public string ShippingAddress { get; set; }

        internal static OrderModel FromDTO(ReadModel.OrderDTO dto)
        {
            return new OrderModel
            {
                Id = dto.Id,
                State = dto.State,
                ShippingAddress = dto.ShippingAddress,
                Lines = dto.OrderLines.Select(LineModel.FromDTO).ToList(),
                ProcessId = dto.ProcessId
            };
        }
    }
}