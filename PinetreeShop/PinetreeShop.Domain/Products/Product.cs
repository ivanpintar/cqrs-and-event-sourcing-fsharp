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
        private string _name;
        private decimal _price;
        private uint _quantity;
        private uint _reserved;
        private uint AvailableQuantity { get { return _quantity - _reserved; } }

        public Product()
        {
            _quantity = 0;
            _reserved = 0;

            RegisterEventHandler<ProductCreated>(Apply);
            RegisterEventHandler<ProductQuantityChanged>(Apply);
            RegisterEventHandler<ProductReserved>(Apply);
            RegisterEventHandler<ProductReservationCancelled>(Apply);
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

        private void Apply(ProductReservationCancelled evt)
        {
            AggregateId = evt.AggregateId;
            _reserved = _reserved - evt.Quantity;
        }

        private void Apply(ProductReserved evt)
        {
            AggregateId = evt.AggregateId;
            _reserved = _reserved + evt.QuantityToReserve;
        }

        private void Apply(ProductQuantityChanged evt)
        {
            AggregateId = evt.AggregateId;
            _quantity = (uint)((int)_quantity + evt.Difference);
        }

        internal void ChangeQuantity(Guid productId, int difference)
        {
            if ((int)_quantity + difference < 0) throw new QuantityChangeException(AggregateId, $"Quantity can't be negative. Quantity: {_quantity}, Diff: {difference}");

            RaiseEvent(new ProductQuantityChanged(productId, difference));
        }

        internal void CancelReservation(Guid productId, uint quantity)
        {
            if (quantity > _reserved) quantity = _reserved;
            RaiseEvent(new ProductReservationCancelled(productId, quantity));
        }

        internal void Reserve(Guid productId, Guid basketId, uint quantity)
        {
            if (AvailableQuantity < quantity)
            {
                RaiseEvent(new ProductReservationFailed(productId, basketId, quantity, ProductReservationFailed.NotAvailable));
            }
            else
            {
                RaiseEvent(new ProductReserved(productId, basketId, quantity));
            }
        }

        private void Apply(ProductCreated evt)
        {
            AggregateId = evt.AggregateId;
            _name = evt.Name;
            _price = evt.Price;
        }
    }
}
