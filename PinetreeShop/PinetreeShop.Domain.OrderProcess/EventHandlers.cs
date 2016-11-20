using System;
using PinetreeShop.Domain.Baskets.Events;
using PinetreeShop.Domain.Products.Events;
using PinetreeShop.Domain.Orders.Events;

namespace PinetreeShop.Domain.OrderProcess
{
    public static class EventHandlers
    {
        public static Func<OrderProcessManager, CreateOrderFailed, OrderProcessManager> CreateOrderFailed = (process, evt) =>
        {
            process.CreateOrderFailed(evt);
            return process;
        };

        public static Func<OrderProcessManager, OrderShipped, OrderProcessManager> OrderShipped = (process, evt) =>
        {
            process.OrderShipped(evt);
            return process;
        };

        public static Func<OrderProcessManager, OrderDelivered, OrderProcessManager> OrderDelivered = (process, evt) =>
        {
            process.OrderDelivered(evt);
            return process;
        };

        public static Func<OrderProcessManager, OrderCancelled, OrderProcessManager> OrderCancelled = (process, evt) =>
        {
            process.OrderCancelled(evt);
            return process;
        };

        public static Func<OrderProcessManager, OrderCreated, OrderProcessManager> OrderCreated = (process, evt) =>
        {
            process.OrderCreated(evt);
            return process;
        };

        public static Func<OrderProcessManager, ProductReservationFailed, OrderProcessManager> ProductReservationFailed = (process, evt) =>
        {
            process.ProductReservationFailed(evt);
            return process;
        };

        public static Func<OrderProcessManager, BasketCheckedOut, OrderProcessManager> BasketCheckedOut = (process, evt) =>
        {
            process.BasketCheckedOut(evt);
            return process;
        };

        public static Func<OrderProcessManager, ProductReserved, OrderProcessManager> ProductReserved = (process, evt) =>
        {
            process.ProductReserved(evt);
            return process;
        };


    }
}
