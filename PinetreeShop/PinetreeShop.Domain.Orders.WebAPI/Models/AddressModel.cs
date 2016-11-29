namespace PinetreeShop.Domain.Orders.WebAPI.Models
{
    public class AddressModel
    {
        public string StreetAndNumber { get; set; }
        public string ZipAndCity { get; set; }
        public string StateOrProvince { get; set; }
        public string Country { get; set; }
    }
}