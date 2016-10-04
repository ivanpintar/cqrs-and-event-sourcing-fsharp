using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.Domain.Baskets.Events;
using PinetreeShop.Domain.Baskets.Exceptions;
using PinetreeShop.Domain.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PinetreeShop.Domain.Baskets
{
    public class Basket : AggregateBase
    {
        public enum BasketState { Pending, Cancelled, CheckedOut };
        public BasketState State { get; private set; }

        private List<OrderLine> _orderLines = new List<OrderLine>();
        public IEnumerable<OrderLine> OrderLines { get { return _orderLines; } }

        private Basket(Guid basketId) : this()
        {
            RaiseEvent(new BasketCreated(basketId));
        }

        public Basket()
        {
            RegisterEventHandler<BasketCreated>(Apply);
            RegisterEventHandler<ProductAdded>(Apply);
            RegisterEventHandler<AddProductReverted>(Apply);
            RegisterEventHandler<ProductRemoved>(Apply);
            RegisterEventHandler<Cancelled>(Apply);
            RegisterEventHandler<CheckedOut>(Apply);
            RegisterEventHandler<CheckOutReverted>(Apply);
        }

        internal void AddProduct(Guid basketId, Guid productId, string productName, decimal price, uint quantity)
        {
            RaiseEvent(new ProductAdded(basketId, productId, productName, price, quantity));
        }

        private void Apply(CheckOutReverted evt)
        {
            Id = evt.AggregateId;
            State = BasketState.Pending;
        }

        private void Apply(AddProductReverted evt)
        {
            Id = evt.AggregateId;
            RemoveProductFromOrderLines(evt.ProductId, evt.Quantity);
        }

        internal void RevertCheckout(Guid basketId, string reason)
        {
            RaiseEvent(new CheckOutReverted(basketId, reason));
        }

        private void Apply(BasketCreated evt)
        {
            Id = evt.AggregateId;
            State = BasketState.Pending;
        }

        private void Apply(ProductAdded evt)
        {
            Id = evt.AggregateId;
            AddProductToOrderLines(evt.ProductId, evt.ProductName, evt.Price, evt.Quantity);
        }

        internal void RevertAddProduct(Guid basketId, Guid productId, uint quantity, string reason)
        {
            RaiseEvent(new AddProductReverted(basketId, productId, quantity, reason));
        }

        private void Apply(ProductRemoved evt)
        {
            Id = evt.AggregateId;
            RemoveProductFromOrderLines(evt.ProductId, evt.Quantity);
        }

        private void Apply(Cancelled evt)
        {
            Id = evt.AggregateId;
            State = BasketState.Cancelled;
        }

        internal static IAggregate Create(Guid basketId)
        {
            return new Basket(basketId);
        }

        internal void RemoveProduct(Guid basketId, Guid productId, uint quantity)
        {
            var orderLine = _orderLines.SingleOrDefault(ol => ol.ProductId == productId);
            if (orderLine != null)
            {
                if (orderLine.Quantity < quantity) quantity = orderLine.Quantity;
                RaiseEvent(new ProductRemoved(basketId, productId, quantity));
            }
        }

        private void Apply(CheckedOut evt)
        {
            Id = evt.AggregateId;
            State = BasketState.CheckedOut;
        }

        internal void Cancel(Guid basketId)
        {
            if (State == BasketState.Cancelled) return;

            if (State != BasketState.Pending) throw new CancellationException(basketId, $"Cannot cancel, basket is {State}");
            RaiseEvent(new Cancelled(basketId));
        }

        internal void CheckOut(Guid basketId, Address shippingAddress)
        {
            if (State != BasketState.Pending) throw new CheckoutException(basketId, $"Cannot check out, basket is {State}");
            RaiseEvent(new CheckedOut(basketId, shippingAddress));
        }

        private void AddProductToOrderLines(Guid productId, string productName, decimal price, uint quantity)
        {
            var orderLine = _orderLines.SingleOrDefault(ol => ol.ProductId == productId);
            if (orderLine == null)
            {
                orderLine = new OrderLine
                {
                    ProductId = productId,
                    ProductName = productName,
                    Price = price,
                    Quantity = quantity
                };
                _orderLines.Add(orderLine);
            }
            else
            {
                orderLine.Quantity += quantity;
            }
        }

        private void RemoveProductFromOrderLines(Guid productId, uint quantity)
        {
            var orderLine = _orderLines.Single(ol => ol.ProductId == productId);
            orderLine.Quantity = orderLine.Quantity - quantity;

            if (orderLine.Quantity == 0)
            {
                _orderLines.Remove(orderLine);
            }
        }
    }
}
