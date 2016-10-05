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
            RegisterEventHandler<BasketAddItemTried>(Apply);
            RegisterEventHandler<BasketAddItemConfirmed>(Apply);
            RegisterEventHandler<BasketAddItemReverted>(Apply);
            RegisterEventHandler<BasketItemRemoved>(Apply);
            RegisterEventHandler<BaksetCancelled>(Apply);
            RegisterEventHandler<BasketCheckedOut>(Apply);
        }

        private void Apply(BasketAddItemConfirmed evt)
        {
            AggregateId = evt.AggregateId;
            AddProductToOrderLines(evt.ProductId, evt.Quantity);
        }

        internal void TryAddProduct(Guid basketId, Guid productId, string productName, decimal price, uint quantity)
        {
            RaiseEvent(new BasketAddItemTried(basketId, productId, productName, price, quantity));
        }

        private void Apply(BasketAddItemReverted evt)
        {
            AggregateId = evt.AggregateId;
            RemoveProductFromOrderLines(evt.ProductId, evt.Quantity);
        }

        internal void ConfirmAddItem(Guid basketId, Guid productId, uint quantity)
        {
            RaiseEvent(new BasketAddItemConfirmed(basketId, productId, quantity));
        }

        private void Apply(BasketCreated evt)
        {
            AggregateId = evt.AggregateId;
            State = BasketState.Pending;
        }

        private void Apply(BasketAddItemTried evt)
        {
            AggregateId = evt.AggregateId;
            AddProductToOrderLines(evt.ProductId, 0, evt.ProductName, evt.Price); // do not add quantity until items are reserved
        }

        internal void RevertAddProduct(Guid basketId, Guid productId, uint quantity, string reason)
        {
            RaiseEvent(new BasketAddItemReverted(basketId, productId, quantity, reason));
        }

        private void Apply(BasketItemRemoved evt)
        {
            AggregateId = evt.AggregateId;
            RemoveProductFromOrderLines(evt.ProductId, evt.Quantity);
        }

        private void Apply(BaksetCancelled evt)
        {
            AggregateId = evt.AggregateId;
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
                RaiseEvent(new BasketItemRemoved(basketId, productId, quantity));
            }
        }

        private void Apply(BasketCheckedOut evt)
        {
            AggregateId = evt.AggregateId;
            State = BasketState.CheckedOut;
        }

        internal void Cancel(Guid basketId)
        {
            if (State == BasketState.Cancelled) return;

            if (State != BasketState.Pending) throw new CancellationException(basketId, $"Cannot cancel, basket is {State}");
            RaiseEvent(new BaksetCancelled(basketId));
        }

        internal void TryCheckOut(Guid basketId, Address shippingAddress)
        {
            if (State != BasketState.Pending) throw new CheckoutException(basketId, $"Cannot check out, basket is {State}");
            RaiseEvent(new BasketCheckedOut(basketId, shippingAddress));
        }

        private void AddProductToOrderLines(Guid productId, uint quantity, string productName = "", decimal? price = null)
        {
            var orderLine = _orderLines.SingleOrDefault(ol => ol.ProductId == productId);
            if (orderLine == null)
            {
                orderLine = new OrderLine
                {
                    ProductId = productId,
                    ProductName = productName,
                    Price = price.Value,
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
