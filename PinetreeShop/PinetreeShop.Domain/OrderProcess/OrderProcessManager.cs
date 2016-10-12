using PinetreeShop.CQRS.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PinetreeShop.Domain.Baskets.Events;
using PinetreeShop.Domain.Products.Commands;
using PinetreeShop.Domain.Products.Events;
using PinetreeShop.Domain.Baskets.Commands;
using PinetreeShop.Domain.Baskets;
using PinetreeShop.Domain.Orders.Commands;
using PinetreeShop.Domain.Types;
using PinetreeShop.CQRS.Infrastructure.Repositories;

namespace PinetreeShop.Domain.OrderProcess
{
    public class OrderProcessManager : ProcessManagerBase
    {
        private Guid _basketId;
        private Dictionary<Guid, bool> _reservations = new Dictionary<Guid, bool>();
        private List<OrderLine> _orderLines = new List<OrderLine>();
        private Address _shippingAddress;

        public OrderProcessManager()
        {
            RegisterEventHandler<BasketCheckedOut>(Apply);
            RegisterEventHandler<ProductReserved>(Apply);
        }

        internal void BasketCheckedOut(BasketCheckedOut evt)
        {
            HandleEvent(evt);
        }

        private void Apply(BasketCheckedOut evt)
        {
            ProcessId = evt.Metadata.CorrelationId;
            _basketId = evt.AggregateId;

            BuildOrderLines();
            _shippingAddress = evt.ShippingAddress;

            foreach (var orderLine in _orderLines)
            {
                _reservations[orderLine.ProductId] = false;
                DispatchCommand(new ReserveProduct(orderLine.ProductId, _basketId, orderLine.Quantity));
            }
        }

        internal void ProductReserved(ProductReserved evt)
        {
            HandleEvent(evt);
        }

        private void Apply(ProductReserved evt)
        {
            _reservations[evt.AggregateId] = true;

            if (_reservations.Values.All(v => v))
            {
                DispatchCommand(new CreateOrder(AggregateRepositoryBase.CreateGuid(), _basketId, _orderLines, _shippingAddress));
            }
        }

        private void BuildOrderLines()
        {
            var orderEvents = _events.Where(e => e.AggregateId == _basketId && (e is BasketItemAdded || e is BasketItemRemoved)).ToList();

            foreach (var oe in orderEvents)
            {
                if (oe is BasketItemAdded)
                    AddProductToOrderLines(oe as BasketItemAdded);
                else
                    RemoveProductFromOrderLines(oe as BasketItemRemoved);
            }
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
