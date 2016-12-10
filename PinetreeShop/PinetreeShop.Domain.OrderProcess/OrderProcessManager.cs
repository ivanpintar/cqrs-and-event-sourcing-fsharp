using PinetreeShop.CQRS.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using PinetreeShop.Domain.Baskets.Events;
using PinetreeShop.Domain.Products.Commands;
using PinetreeShop.Domain.Products.Events;
using PinetreeShop.Domain.Orders.Commands;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.Domain.Orders;
using PinetreeShop.Domain.Orders.Events;
using PinetreeShop.Domain.OrderProcess.Commands;
using PinetreeShop.Domain.Shared.Types;
using PinetreeShop.Domain.Products;

namespace PinetreeShop.Domain.OrderProcess
{
    public class OrderProcessManager : ProcessManagerBase
    {
        private Guid _basketId;
        private Dictionary<Guid, bool> _reservations = new Dictionary<Guid, bool>();
        private Dictionary<Guid, OrderLine> _orderLines = new Dictionary<Guid, OrderLine>();
        private Address _shippingAddress;
        private Guid _orderId;

        public OrderProcessManager()
        {
            RegisterEventHandler<BasketCheckedOut>(Apply);
            RegisterEventHandler<ProductReserved>(Apply);
            RegisterEventHandler<ProductReservationFailed>(Apply);
            RegisterEventHandler<OrderCreated>(Apply);
            RegisterEventHandler<CreateOrderFailed>(Apply);
            RegisterEventHandler<OrderShipped>(Apply);
            RegisterEventHandler<OrderDelivered>(Apply);
            RegisterEventHandler<OrderCancelled>(Apply);
        }

        #region Internal Event handlers

        private void Apply(BasketCheckedOut evt)
        {
            ProcessId = evt.Metadata.ProcessId;
            _orderId = ProcessId;
            _basketId = evt.AggregateId;
            _shippingAddress = evt.ShippingAddress;
            _orderLines = evt.OrderLines.ToDictionary(ol => ol.ProductId, ol => ol);
            
            DispatchCommand<OrderAggregate>(new CreateOrder(_orderId, _basketId, _shippingAddress));
        }

        private void Apply(ProductReserved evt)
        {
            _reservations[evt.AggregateId] = true;

            var orderLine = _orderLines[evt.AggregateId];
            DispatchCommand<OrderAggregate>(new AddOrderLine(_orderId, orderLine));

            if(_reservations.All(x => x.Value))
            {
                DispatchCommand<OrderAggregate>(new PrepareOrderForShipping(_orderId));
            }
        }

        private void Apply(ProductReservationFailed obj)
        {
            DispatchCommand<DummyNotifier>(new NotifyAdmin(AggregateRepository.CreateGuid()));
        }

        private void Apply(OrderCreated evt)
        {
            _orderId = evt.AggregateId;

            foreach (var orderLine in _orderLines.Values)
            {
                _reservations[orderLine.ProductId] = false;
                DispatchCommand<ProductAggregate>(new ReserveProduct(orderLine.ProductId, orderLine.Quantity));
            }
        }

        private void Apply(CreateOrderFailed obj)
        {
            DispatchCommand<DummyNotifier>(new NotifyAdmin(AggregateRepository.CreateGuid()));
        }

        private void Apply(OrderCancelled obj)
        {
            var reservedOrders = _orderLines.Values.Where(ol => _reservations.Any(r => r.Key == ol.ProductId && r.Value));
            foreach (var ol in reservedOrders)
            {
                DispatchCommand<ProductAggregate>(new CancelProductReservation(ol.ProductId, ol.Quantity));
            }
            DispatchCommand<DummyNotifier>(new NotifyCustomer(AggregateRepository.CreateGuid()));
        }

        private void Apply(OrderShipped evt)
        {
            var reservedOrders = _orderLines.Values.Where(ol => _reservations.Any(r => r.Key == ol.ProductId && r.Value));
            foreach (var ol in reservedOrders)
            {
                DispatchCommand<ProductAggregate>(new PurchaseReservedProduct(ol.ProductId, ol.Quantity));
            }
            DispatchCommand<DummyNotifier>(new NotifyCustomer(AggregateRepository.CreateGuid()));
        }

        private void Apply(OrderDelivered obj)
        {
            DispatchCommand<DummyNotifier>(new NotifyAdmin(AggregateRepository.CreateGuid()));
        }

        #endregion

        #region External Event Handlers

        internal void BasketCheckedOut(BasketCheckedOut evt)
        {
            HandleEvent(evt);
        }

        internal void ProductReserved(ProductReserved evt)
        {
            HandleEvent(evt);
        }

        internal void ProductReservationFailed(ProductReservationFailed evt)
        {
            HandleEvent(evt);
        }

        internal void OrderCreated(OrderCreated evt)
        {
            HandleEvent(evt);
        }

        internal void CreateOrderFailed(CreateOrderFailed evt)
        {
            HandleEvent(evt);
        }

        internal void OrderCancelled(OrderCancelled evt)
        {
            HandleEvent(evt);
        }

        internal void OrderShipped(OrderShipped evt)
        {
            HandleEvent(evt);
        }

        internal void OrderDelivered(OrderDelivered evt)
        {
            HandleEvent(evt);
        }

        #endregion
    }
}
