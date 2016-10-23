using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.Domain.Products.Commands;
using PinetreeShop.Domain.Products.Events;
using PinetreeShop.Domain.Products.Exceptions;

namespace PinetreeShop.Domain.Products
{
    public class ProductAggregate : AggregateBase
    {
        private string _name;
        private decimal _price;
        private int _quantity;
        private int _reserved;
        private int AvailableQuantity { get { return _quantity - _reserved; } }

        public ProductAggregate()
        {
            _quantity = 0;
            _reserved = 0;

            RegisterEventHandler<ProductCreated>(Apply);
            RegisterEventHandler<ProductQuantityChanged>(Apply);
            RegisterEventHandler<ProductReserved>(Apply);
            RegisterEventHandler<ProductReservationCancelled>(Apply);
        }

        private ProductAggregate(CreateProduct cmd) : this()
        {
            RaiseEvent(new ProductCreated(cmd.AggregateId, cmd.Name, cmd.Price));
        }

        internal static ProductAggregate Create(CreateProduct cmd)
        {
            if (cmd.Price <= 0) throw new ProductCreationException(cmd.AggregateId, $"Price {cmd.Price} must be a positive value.");

            return new ProductAggregate(cmd);
        }

        private void Apply(ProductReservationCancelled evt)
        {
            _reserved -= evt.Quantity;
        }

        private void Apply(ProductReserved evt)
        {
            _reserved += evt.QuantityToReserve;
        }

        private void Apply(ProductQuantityChanged evt)
        {
            _quantity += evt.Difference;
        }

        internal void ChangeQuantity(ChangeProductQuantity cmd)
        {
            var productId = cmd.AggregateId;
            var difference = cmd.Difference;

            if (_quantity + difference < 0) throw new QuantityChangeException(AggregateId, $"Quantity can't be negative. Quantity: {_quantity}, Diff: {difference}");

            RaiseEvent(new ProductQuantityChanged(productId, difference));
        }

        internal void CancelReservation(CancelProductReservation cmd)
        {
            var productId = cmd.AggregateId;
            var quantity = cmd.Quantity;

            if (quantity > _reserved) quantity = _reserved;
            RaiseEvent(new ProductReservationCancelled(productId, quantity));
        }

        internal void Reserve(ReserveProduct cmd)
        {
            var productId = cmd.AggregateId;
            var basketId = cmd.BasketId;
            var quantity = cmd.Quantity;

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
