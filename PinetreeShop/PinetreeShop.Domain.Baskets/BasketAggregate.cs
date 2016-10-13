using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.Domain.Baskets.Events;
using PinetreeShop.Domain.Baskets.Commands;
using PinetreeShop.Domain.Baskets.Exceptions;
using System.Collections.Generic;
using System.Linq;
using PinetreeShop.Domain.Shared.Types;

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
            RegisterEventHandler<BasketItemAdded>(Apply);
            RegisterEventHandler<BasketItemRemoved>(Apply);
            RegisterEventHandler<BasketCancelled>(Apply);
            RegisterEventHandler<BasketCheckedOut>(Apply);
        }

        internal static IAggregate Create(CreateBasket cmd)
        {
            return new BasketAggregate(cmd);
        }

        private void Apply(BasketCreated evt)
        {
            AggregateId = evt.AggregateId;
            _state = BasketState.Pending;
        }

        internal void AddItemToBasket(AddItemToBasket cmd)
        {
            if (_state != BasketState.Pending)
                throw new InvalidStateException(AggregateId, $"Cannot add item. Basket is {_state}");

            RaiseEvent(new BasketItemAdded(cmd.AggregateId, cmd.ProductId, cmd.ProductName, cmd.Price, cmd.Quantity));
        }

        private void Apply(BasketItemAdded evt)
        {
            AddProductToOrderLines(evt);
        }

        internal void RemoveItemFromBasket(RemoveItemFromBasket cmd)
        {
            if (_state != BasketState.Pending) throw new InvalidStateException(AggregateId, $"Cannot remove item. Basket is {_state}");

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

        private void Apply(BasketItemRemoved evt)
        {
            RemoveProductFromOrderLines(evt);
        }

        internal void Cancel(CancelBasket cmd)
        {
            if (_state == BasketState.Cancelled) return;

            if (_state != BasketState.Pending)
                throw new InvalidStateException(cmd.AggregateId, $"Cannot cancel, basket is {_state}");

            RaiseEvent(new BasketCancelled(cmd.AggregateId));
        }

        private void Apply(BasketCancelled evt)
        {
            _state = BasketState.Cancelled;
        }

        internal void CheckOut(CheckOutBasket cmd)
        {
            if (_state == BasketState.CheckedOut || !_orderLines.Any()) return;

            if (_state != BasketState.Pending)
                throw new InvalidStateException(cmd.AggregateId, $"Cannot check out, basket is {_state}");

            RaiseEvent(new BasketCheckedOut(cmd.AggregateId, cmd.ShippingAddress));
        }

        private void Apply(BasketCheckedOut evt)
        {
            _state = BasketState.CheckedOut;
        }

        private void AddProductToOrderLines(BasketItemAdded evt)
        {
            var productId = evt.ProductId;
            var productName = evt.ProductName;
            var price = evt.Price;
            var quantity = evt.Quantity;
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

        private void RemoveProductFromOrderLines(BasketItemRemoved evt)
        {
            var productId = evt.ProductId;
            var quantity = evt.Quantity;
            var orderLine = _orderLines.Single(ol => ol.ProductId == productId);
            orderLine.Quantity = orderLine.Quantity - quantity;

            if (orderLine.Quantity == 0)
            {
                _orderLines.Remove(orderLine);
            }
        }
    }
}
