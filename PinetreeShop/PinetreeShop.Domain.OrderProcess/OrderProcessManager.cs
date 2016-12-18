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
        private OrderAggregate.OrderState _state;

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
            // todo: if order not pending revoke reservation
            if(_state != OrderAggregate.OrderState.Pending)
            {
                DispatchCommand<ProductAggregate>(new CancelProductReservation(evt.AggregateId, evt.QuantityToReserve));
                return;
            }

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
            _state = OrderAggregate.OrderState.Pending;

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
            _state = OrderAggregate.OrderState.Cancelled;

            var reservedOrders = _orderLines.Values.Where(ol => _reservations.Any(r => r.Key == ol.ProductId && r.Value));
            foreach (var ol in reservedOrders)
            {
                DispatchCommand<ProductAggregate>(new CancelProductReservation(ol.ProductId, ol.Quantity));
            }
            DispatchCommand<DummyNotifier>(new NotifyCustomer(AggregateRepository.CreateGuid()));
        }

        private void Apply(OrderShipped evt)
        {
            _state = OrderAggregate.OrderState.Shipped;
            var reservedOrders = _orderLines.Values.Where(ol => _reservations.Any(r => r.Key == ol.ProductId && r.Value));
            foreach (var ol in reservedOrders)
            {
                DispatchCommand<ProductAggregate>(new PurchaseReservedProduct(ol.ProductId, ol.Quantity));
            }
            DispatchCommand<DummyNotifier>(new NotifyCustomer(AggregateRepository.CreateGuid()));
        }

        private void Apply(OrderDelivered obj)
        {
            _state = OrderAggregate.OrderState.Delivered;
            DispatchCommand<DummyNotifier>(new NotifyAdmin(AggregateRepository.CreateGuid()));
        }

        #endregion
    }
}
