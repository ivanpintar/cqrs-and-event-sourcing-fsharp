using System;
using static PinetreeShop.Domain.Orders.ReadModel;

namespace PinetreeShop.Domain.Orders.WebAPI.Models
{
    public class LineModel
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool Reserved { get; set; }

        internal static LineModel FromDTO(OrderLineDTO dto)
        {
            return new LineModel
            {
                ProductId = dto.ProductId,
                ProductName = dto.ProductName,
                Price = dto.Price,
                Quantity = dto.Quantity,
                Reserved = dto.Reserved
            };
        }
    }
}