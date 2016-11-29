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

        internal void BasketCheckedOut(BasketCheckedOut evt)
        {
            HandleEvent(evt);
        }

        private void Apply(BasketCheckedOut evt)
        {
            ProcessId = evt.Metadata.CorrelationId;
            _basketId = evt.AggregateId;
            _shippingAddress = evt.ShippingAddress;
            _orderLines = evt.OrderLines.ToDictionary(ol => ol.ProductId, ol => ol);
            
            DispatchCommand<OrderAggregate>(new CreateOrder(AggregateRepositoryBase.CreateGuid(), _basketId, _shippingAddress, ProcessId));
        }

        internal void ProductReserved(ProductReserved evt)
        {
            HandleEvent(evt);
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

        internal void ProductReservationFailed(ProductReservationFailed evt)
        {
            HandleEvent(evt);
        }

        private void Apply(ProductReservationFailed obj)
        {
            DispatchCommand<DummyNotifier>(new NotifyAdmin(AggregateRepositoryBase.CreateGuid()));
        }

        internal void OrderCreated(OrderCreated evt)
        {
            HandleEvent(evt);
        }

        private void Apply(OrderCreated evt)
        {
            _orderId = evt.AggregateId;

            foreach (var orderLine in _orderLines.Values)
            {
                _reservations[orderLine.ProductId] = false;
                DispatchCommand<ProductAggregate>(new ReserveProduct(orderLine.ProductId, _basketId, orderLine.Quantity));
            }
        }

        internal void CreateOrderFailed(CreateOrderFailed evt)
        {
            HandleEvent(evt);
        }

        private void Apply(CreateOrderFailed obj)
        {
            DispatchCommand<DummyNotifier>(new NotifyAdmin(AggregateRepositoryBase.CreateGuid()));
        }

        internal void OrderCancelled(OrderCancelled evt)
        {
            HandleEvent(evt);
        }

        private void Apply(OrderCancelled obj)
        {
            foreach (var ol in _orderLines.Values)
            {
                DispatchCommand<ProductAggregate>(new CancelProductReservation(ol.ProductId, ol.Quantity));
            }
            DispatchCommand<DummyNotifier>(new NotifyCustomer(AggregateRepositoryBase.CreateGuid()));
        }

        internal void OrderShipped(OrderShipped evt)
        {
            HandleEvent(evt);
        }

        private void Apply(OrderShipped evt)
        {
            foreach(var ol in _orderLines.Values)
            {
                DispatchCommand<ProductAggregate>(new RemoveProductFromStock(ol.ProductId, ol.Quantity));
            }
            DispatchCommand<DummyNotifier>(new NotifyCustomer(AggregateRepositoryBase.CreateGuid()));
        }

        internal void OrderDelivered(OrderDelivered evt)
        {
            HandleEvent(evt);
        }

        private void Apply(OrderDelivered obj)
        {
            DispatchCommand<DummyNotifier>(new NotifyAdmin(AggregateRepositoryBase.CreateGuid()));
        }
    }
}
