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

        private Product(Guid productId, string name, decimal price) : this()
        {
            RaiseEvent(new ProductCreated(productId, name, price));
        }

        internal static IAggregate Create(Guid productId, string name, decimal price)
        {
            if (price <= 0) throw new ProductCreationException(productId, $"Price {price} must be a positive value.");

            return new Product(productId, name, price);
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

        internal void ChangeQuantity(Guid productId, int difference)
        {
            if ((int)Quantity + difference < 0) throw new QuantityChangeException(Id, $"Quantity can't be negative. Quantity: {Quantity}, Diff: {difference}");

            RaiseEvent(new ProductQuantityChanged(productId, difference));
        }

        internal void ReleaseReservation(Guid productId, uint quantityToRelease)
        {
            if (quantityToRelease > Reserved) quantityToRelease = Reserved;
            RaiseEvent(new ProductReservationReleased(productId, quantityToRelease));
        }

        internal void Reserve(Guid productId, Guid basketId, uint quantityToReserve)
        {
            if (AvailableQuantity < quantityToReserve)
            {
                RaiseEvent(new ProductReservationFailed(productId, basketId, quantityToReserve, ProductReservationFailed.NotAvailable));
            }
            else
            {
                RaiseEvent(new ProductReserved(productId, basketId, quantityToReserve));
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
