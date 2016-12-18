using System;
using PinetreeCQRS.Infrastructure.Commands;
using PinetreeCQRS.Infrastructure.Repositories;
using PinetreeShop.Domain.Orders.Commands;
using PinetreeShop.Domain.Shared.Exceptions;

namespace PinetreeShop.Domain.Orders
{
    public class OrderCommandDispatcher : CommandDispatcher
    {
        public OrderCommandDispatcher(IAggregateRepository aggregateRepository) : base(aggregateRepository)
        {
            RegisterHandler(Create);
            RegisterHandler(AddOrderLine);
            RegisterHandler(PrepareForShipping);
            RegisterHandler(Cancel);
            RegisterHandler(Ship);
            RegisterHandler(Deliver);
        }

        private Func<OrderAggregate, PrepareOrderForShipping, OrderAggregate> PrepareForShipping = (order, command) =>
        {
            command.Metadata.ProcessId = order.AggregateId;
            order.PrepareForShipping(command);
            return order;
        };

        private Func<OrderAggregate, AddOrderLine, OrderAggregate> AddOrderLine = (order, command) =>
        {
            command.Metadata.ProcessId = order.AggregateId;
            order.AddOrderLine(command);
            return order;
        };

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
            command.Metadata.ProcessId = order.AggregateId;
            order.Cancel(command);
            return order;
        };

        private Func<OrderAggregate, ShipOrder, OrderAggregate> Ship = (order, command) =>
        {
            command.Metadata.ProcessId = order.AggregateId;
            order.Ship(command);
            return order;
        };

        private Func<OrderAggregate, DeliverOrder, OrderAggregate> Deliver = (order, command) =>
        {
            command.Metadata.ProcessId = order.AggregateId;
            order.Deliver(command);
            return order;
        };
    }
}
