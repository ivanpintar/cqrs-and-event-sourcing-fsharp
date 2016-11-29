using System;

namespace PinetreeShop.Domain.Orders.ReadModel.Entitites
{
    public class Line
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
