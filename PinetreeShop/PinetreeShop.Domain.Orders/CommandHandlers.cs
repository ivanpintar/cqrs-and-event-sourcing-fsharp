using PinetreeShop.Domain.Orders.Commands;
using PinetreeShop.Domain.Shared.Exceptions;
using System;
using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.Commands;

namespace PinetreeShop.Domain.Orders
{
    public static class CommandHandlers 
    {
        public static Func<OrderAggregate, CreateOrder, OrderAggregate> Create = (order, command) =>
        {
            if (order != null)
            {
                throw new AggregateExistsException(order.AggregateId, "Product already exists");
            }
            return OrderAggregate.Create(command);
        };

        public static Func<OrderAggregate, CancelOrder, OrderAggregate> Cancel = (order, command) =>
        {
            order.Cancel(command);
            return order;
        };

        public static Func<OrderAggregate, ShipOrder, OrderAggregate> Ship = (order, command) =>
        {
            order.Ship(command);
            return order;
        };

        public static Func<OrderAggregate, DeliverOrder, OrderAggregate> Deliver = (order, command) =>
        {
            order.Deliver(command);
            return order;
        };
    }
}
