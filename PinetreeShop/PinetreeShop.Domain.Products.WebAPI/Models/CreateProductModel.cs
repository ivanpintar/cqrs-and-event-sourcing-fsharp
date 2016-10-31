namespace PinetreeShop.Domain.Products.WebAPI.Models
{
    public class CreateProductModel
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}