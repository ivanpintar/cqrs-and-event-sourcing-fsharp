using System;
using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.Domain.Orders.Commands;
using PinetreeShop.Domain.Shared.Exceptions;

namespace PinetreeShop.Domain.Orders
{
    public class OrderCommandDispatcher : CommandDispatcher
    {
        public OrderCommandDispatcher(IAggregateRepository aggregateRepository) : base(aggregateRepository)
        {
            RegisterHandler(Create);
            RegisterHandler(Cancel);
            RegisterHandler(Ship);
            RegisterHandler(Deliver);
        }

        private Func<OrderAggregate, CreateOrder, OrderAggregate> Create = (order, command) =>
        {
            if (order != null)
            {
                throw new AggregateExistsException(order.AggregateId, "Product already exists");
            }
            return OrderAggregate.Create(command);
        };

        private Func<OrderAggregate, CancelOrder, OrderAggregate> Cancel = (order, command) =>
        {
            order.Cancel(command);
            return order;
        };

        private Func<OrderAggregate, ShipOrder, OrderAggregate> Ship = (order, command) =>
        {
            order.Ship(command);
            return order;
        };

        private Func<OrderAggregate, DeliverOrder, OrderAggregate> Deliver = (order, command) =>
        {
            order.Deliver(command);
            return order;
        };
    }
}
