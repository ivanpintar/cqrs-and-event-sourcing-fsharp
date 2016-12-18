using System;
using PinetreeCQRS.Infrastructure;
using PinetreeShop.Domain.Products.Commands;
using PinetreeShop.Domain.Products.Events;
using PinetreeShop.Domain.Products.Exceptions;

namespace PinetreeShop.Domain.Products
{
    public class ProductAggregate : AggregateBase
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int Reserved { get; set; }
        private int AvailableQuantity { get { return Quantity - Reserved; } }

        #region Constructors

        public ProductAggregate()
        {
            Quantity = 0;
            Reserved = 0;

            RegisterEventHandler<ProductCreated>(Apply);
            RegisterEventHandler<ProductQuantityChanged>(Apply);
            RegisterEventHandler<ProductReserved>(Apply);
            RegisterEventHandler<ProductReservationCancelled>(Apply);
            RegisterEventHandler<ReservedProductPurchased>(Apply);
        }

        private ProductAggregate(CreateProduct cmd) : this()
        {
            RaiseEvent(new ProductCreated(cmd.AggregateId, cmd.Name, cmd.Price));
        }

        #endregion

        #region Event handlers

        private void Apply(ReservedProductPurchased obj)
        {
            Quantity -= obj.Quantity;
            Reserved -= obj.Quantity;
        }

        private void Apply(ProductReservationCancelled evt)
        {
            Reserved -= evt.Quantity;
        }

        private void Apply(ProductReserved evt)
        {
            Reserved += evt.QuantityToReserve;
        }

        private void Apply(ProductQuantityChanged evt)
        {
            Quantity += evt.Difference;
        }

        private void Apply(ProductCreated evt)
        {
            AggregateId = evt.AggregateId;
            Name = evt.Name;
            Price = evt.Price;
        }

        #endregion

        #region Command handlers

        internal static ProductAggregate Create(CreateProduct cmd)
        {
            if (cmd.Price <= 0) throw new ProductCreationException(cmd.AggregateId, $"Price {cmd.Price} must be a positive value.");

            return new ProductAggregate(cmd);
        }

        internal void AddToStock(AddProductToStock cmd)
        {
            var newQuantity = Quantity + cmd.Quantity;
            ChangeQuantity(newQuantity);
        }

        internal void PurchaseReserved(PurchaseReservedProduct cmd)
        {
            // TODO: throw exception if reserved or available less than quantity

            RaiseEvent(new ReservedProductPurchased(cmd.AggregateId, cmd.Quantity));
        }

        internal void RemoveFromStock(RemoveProductFromStock cmd)
        {
            var newQuantity = Quantity - cmd.Quantity;
            ChangeQuantity(newQuantity);
        }

        internal void SetQuantity(SetProductQuantity cmd)
        {
            ChangeQuantity(cmd.Quantity);
        }

        internal void CancelReservation(CancelProductReservation cmd)
        {
            var productId = cmd.AggregateId;
            var quantity = cmd.Quantity;

            if (quantity > Reserved) quantity = Reserved;
            RaiseEvent(new ProductReservationCancelled(productId, quantity));
        }

        internal void Reserve(ReserveProduct cmd)
        {
            var productId = cmd.AggregateId;
            var quantity = cmd.Quantity;

            if (AvailableQuantity < quantity)
            {
                RaiseEvent(new ProductReservationFailed(productId, quantity, ProductReservationFailed.NotAvailable));
            }
            else
            {
                RaiseEvent(new ProductReserved(productId, quantity));
            }
        }

        #endregion

        #region Helpers

        private void ChangeQuantity(int newQuantity)
        {
            var productId = AggregateId;
            var difference = newQuantity - Quantity;

            if (difference == 0) return;

            if (Quantity + difference < 0) throw new QuantityChangeException(AggregateId, $"Quantity can't be negative. Quantity: {Quantity}, Diff: {difference}");

            RaiseEvent(new ProductQuantityChanged(productId, difference));
        }

        #endregion
    }
}
