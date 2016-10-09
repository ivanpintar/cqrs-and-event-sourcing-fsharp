using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.Domain.Baskets.Events;
using PinetreeShop.Domain.Baskets.Commands;
using PinetreeShop.Domain.Baskets.Exceptions;
using PinetreeShop.Domain.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PinetreeShop.Domain.Baskets
{
    public class BasketAggregate : AggregateBase
    {
        private enum BasketState { Pending, Cancelled, CheckedOut };
        private BasketState _state;
        private List<OrderLine> _orderLines = new List<OrderLine>();

        private BasketAggregate(CreateBasket cmd) : this()
        {
            RaiseEvent(new BasketCreated(cmd.AggregateId));
        }

        public BasketAggregate()
        {
            RegisterEventHandler<BasketCreated>(Apply);
            RegisterEventHandler<BasketAddItemTried>(Apply);
            RegisterEventHandler<BasketAddItemConfirmed>(Apply);
            RegisterEventHandler<BasketAddItemReverted>(Apply);
            RegisterEventHandler<BasketItemRemoved>(Apply);
            RegisterEventHandler<BasketCancelled>(Apply);
            RegisterEventHandler<BasketCheckedOut>(Apply);
        }

        private void Apply(BasketAddItemConfirmed evt)
        {
            AddProductToOrderLines(evt.ProductId, evt.Quantity);
        }

        internal void TryAddItemToBasket(TryAddItemToBasket cmd)
        {
            RaiseEvent(new BasketAddItemTried(cmd.AggregateId, cmd.ProductId, cmd.ProductName, cmd.Price, cmd.Quantity));
        }

        private void Apply(BasketAddItemReverted evt)
        {
            RemoveProductFromOrderLines(evt.ProductId, evt.Quantity);
        }

        internal void ConfirmAddItem(ConfirmAddItemToBasket cmd)
        {
            RaiseEvent(new BasketAddItemConfirmed(cmd.AggregateId, cmd.ProductId, cmd.Quantity));
        }

        private void Apply(BasketCreated evt)
        {
            AggregateId = evt.AggregateId;
            _state = BasketState.Pending;
        }

        private void Apply(BasketAddItemTried evt)
        {
            AddProductToOrderLines(evt.ProductId, 0, evt.ProductName, evt.Price); // do not add quantity until items are reserved
        }

        internal void RevertAddProduct(RevertAddItemToBasket cmd)
        {
            RaiseEvent(new BasketAddItemReverted(cmd.AggregateId, cmd.ProductId, cmd.Quantity, cmd.Reason));
        }

        private void Apply(BasketItemRemoved evt)
        {
            RemoveProductFromOrderLines(evt.ProductId, evt.Quantity);
        }

        private void Apply(BasketCancelled evt)
        {
            _state = BasketState.Cancelled;
        }

        internal static IAggregate Create(CreateBasket cmd)
        {
            return new BasketAggregate(cmd);
        }

        internal void RemoveItemFromBasket(RemoveItemFromBasket cmd)
        {
            var basketId = cmd.AggregateId;
            var productId = cmd.ProductId;
            var quantity = cmd.Quantity;

            var orderLine = _orderLines.SingleOrDefault(ol => ol.ProductId == productId);
            if (orderLine != null)
            {
                if (orderLine.Quantity < quantity) quantity = orderLine.Quantity;
                RaiseEvent(new BasketItemRemoved(basketId, productId, quantity));
            }
        }

        private void Apply(BasketCheckedOut evt)
        {
            _state = BasketState.CheckedOut;
        }

        internal void Cancel(CancelBasket cmd)
        {
            if (_state == BasketState.Cancelled) return;

            if (_state != BasketState.Pending) throw new CancellationException(cmd.AggregateId, $"Cannot cancel, basket is {_state}");
            RaiseEvent(new BasketCancelled(cmd.AggregateId));
        }

        internal void CheckOut(CheckOutBasket cmd)
        {
            if (_state != BasketState.Pending) throw new CheckoutException(cmd.AggregateId, $"Cannot check out, basket is {_state}");
            RaiseEvent(new BasketCheckedOut(cmd.AggregateId, cmd.ShippingAddress));
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
