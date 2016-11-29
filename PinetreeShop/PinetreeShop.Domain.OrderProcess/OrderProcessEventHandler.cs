using PinetreeShop.CQRS.Infrastructure.Events;
using System;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.Domain.Orders.Events;
using PinetreeShop.Domain.Products.Events;
using PinetreeShop.Domain.Baskets.Events;

namespace PinetreeShop.Domain.OrderProcess
{
    public class OrderProcessEventHandler : ProcessEventHandler
    {
        public OrderProcessEventHandler(IProcessManagerRepository processManagerRepository) : base(processManagerRepository)
        {
            RegisterHandler(BasketCheckedOut);
            RegisterHandler(ProductReserved);
            RegisterHandler(ProductReservationFailed);
            RegisterHandler(OrderCreated);
            RegisterHandler(OrderCancelled);
            RegisterHandler(CreateOrderFailed);
            RegisterHandler(OrderDelivered);
            RegisterHandler(OrderShipped);
        }

        private Func<OrderProcessManager, CreateOrderFailed, OrderProcessManager> CreateOrderFailed = (process, evt) =>
        {
            process.CreateOrderFailed(evt);
            return process;
        };

        private Func<OrderProcessManager, OrderShipped, OrderProcessManager> OrderShipped = (process, evt) =>
        {
            process.OrderShipped(evt);
            return process;
        };

        private Func<OrderProcessManager, OrderDelivered, OrderProcessManager> OrderDelivered = (process, evt) =>
        {
            process.OrderDelivered(evt);
            return process;
        };

        private Func<OrderProcessManager, OrderCancelled, OrderProcessManager> OrderCancelled = (process, evt) =>
        {
            process.OrderCancelled(evt);
            return process;
        };

        private Func<OrderProcessManager, OrderCreated, OrderProcessManager> OrderCreated = (process, evt) =>
        {
            process.OrderCreated(evt);
            return process;
        };

        private Func<OrderProcessManager, ProductReservationFailed, OrderProcessManager> ProductReservationFailed = (process, evt) =>
        {
            process.ProductReservationFailed(evt);
            return process;
        };

        private Func<OrderProcessManager, BasketCheckedOut, OrderProcessManager> BasketCheckedOut = (process, evt) =>
        {
            process.BasketCheckedOut(evt);
            return process;
        };

        private Func<OrderProcessManager, ProductReserved, OrderProcessManager> ProductReserved = (process, evt) =>
        {
            process.ProductReserved(evt);
            return process;
        };
    }
}
