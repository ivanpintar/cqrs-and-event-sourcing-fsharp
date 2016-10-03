using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.Domain.Products.Events;
using PinetreeShop.Domain.Products.Exceptions;
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
        public uint Reserved { get; private set; }
        public uint AvailableQuantity { get { return Quantity - Reserved; } }

        public Product()
        {
            Quantity = 0;
            Reserved = 0;

            RegisterEventHandler<ProductCreated>(Apply);
            RegisterEventHandler<ProductQuantityChanged>(Apply);
            RegisterEventHandler<ProductReserved>(Apply);
            RegisterEventHandler<ProductReservationReleased>(Apply);
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
            Id = evt.AggregateId;
            Reserved = Reserved - evt.QuantityToRelease;
        }

        private void Apply(ProductReserved evt)
        {
            Id = evt.AggregateId;
            Reserved = Reserved + evt.QuantityToReserve;
        }

        private void Apply(ProductQuantityChanged evt)
        {
            Id = evt.AggregateId;
            Quantity = (uint)((int)Quantity + evt.Difference);
        }

        internal void ChangeQuantity(int difference)
        {
            if ((int)Quantity + difference < 0) throw new QuantityChangeException(Id, $"Quantity can't be negative. Quantity: {Quantity}, Diff: {difference}");

            RaiseEvent(new ProductQuantityChanged(Id, difference));
        }

        internal void ReleaseReservation(Guid aggregateId, uint quantityToRelease)
        {
            if (quantityToRelease > Reserved) quantityToRelease = Reserved;
            RaiseEvent(new ProductReservationReleased(aggregateId, quantityToRelease));
        }

        internal void Reserve(Guid aggregateId, Guid basketId, uint quantityToReserve)
        {
            if (AvailableQuantity < quantityToReserve)
            {
                RaiseEvent(new ProductReservationFailed(aggregateId, basketId, quantityToReserve, ProductReservationFailed.NotAvailable));
            }
            else
            {
                RaiseEvent(new ProductReserved(aggregateId, basketId, quantityToReserve));
            }
        }

        private void Apply(ProductCreated evt)
        {
            Id = evt.AggregateId;
            Name = evt.Name;
            Price = evt.Price;
        }
    }
}
