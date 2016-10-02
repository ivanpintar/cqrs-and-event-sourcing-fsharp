using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.Domain.Products.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinetreeShop.Domain.Products
{
    public class Product : AggregateBase
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public uint Quantity { get; set; }
        public uint Available { get; set; }

        public Product()
        {
            RegisterTransition<ProductCreated>(Apply);
            RegisterTransition<ProductQuantityChanged>(Apply);
            RegisterTransition<ProductReserved>(Apply);
            RegisterTransition<ProductReservationReleased>(Apply);            
        }

        private Product(Guid id, string name, int price) : this()
        {

        }

        private void Apply(ProductReservationReleased evt)
        {
            throw new NotImplementedException();
        }

        private void Apply(ProductReserved evt)
        {
            throw new NotImplementedException();
        }

        private void Apply(ProductQuantityChanged evt)
        {
            throw new NotImplementedException();
        }

        private void Apply(ProductCreated evt)
        {
            throw new NotImplementedException();
        }
    }
}
