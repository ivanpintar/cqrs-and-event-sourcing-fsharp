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
        public string Name { get; private set; }
        public decimal Price { get; private set; }
        public uint Quantity { get; private set; }
        public uint AvailableQuantity { get; private set; }

        public Product()
        {
            Quantity = 0;
            AvailableQuantity = 0;

            RegisterTransition<ProductCreated>(Apply);
            RegisterTransition<ProductQuantityChanged>(Apply);
            RegisterTransition<ProductReserved>(Apply);
            RegisterTransition<ProductReservationReleased>(Apply);            
        }

        private Product(Guid aggregateId, string name, decimal price) : this()
        {
            RaiseEvent(new ProductCreated(aggregateId, name, price));
        }
        
        internal static IAggregate Create(Guid aggregateId, string name, decimal price)
        {
            return new Product(aggregateId, name, price);
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
            Id = evt.AggregateId;
            Name = evt.Name;
            Price = evt.Price;
        }
    }
}
