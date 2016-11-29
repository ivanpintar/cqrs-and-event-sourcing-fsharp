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
            RegisterHandler(AddOrderLine);
            RegisterHandler(PrepareForShipping);
            RegisterHandler(Cancel);
            RegisterHandler(Ship);
            RegisterHandler(Deliver);
        }

        private Func<OrderAggregate, PrepareOrderForShipping, OrderAggregate> PrepareForShipping = (order, command) =>
        {
            command.Metadata.CorrelationId = order.ProcessId;
            order.PrepareForShipping(command);
            return order;
        };

        private Func<OrderAggregate, AddOrderLine, OrderAggregate> AddOrderLine = (order, command) =>
        {
            command.Metadata.CorrelationId = order.ProcessId;
            order.AddOrderLine(command);
            return order;
        };

        private Func<OrderAggregate, CreateOrder, OrderAggregate> Create = (order, command) =>
        {
            if (order != null)
            {
                throw new AggregateExistsException(order.AggregateId, "Product already exists");
            }
            command.Metadata.CorrelationId = command.ProcessId;
            return OrderAggregate.Create(command);
        };

        private Func<OrderAggregate, CancelOrder, OrderAggregate> Cancel = (order, command) =>
        {
            command.Metadata.CorrelationId = order.ProcessId;
            order.Cancel(command);
            return order;
        };

        private Func<OrderAggregate, ShipOrder, OrderAggregate> Ship = (order, command) =>
        {
            command.Metadata.CorrelationId = order.ProcessId;
            order.Ship(command);
            return order;
        };

        private Func<OrderAggregate, DeliverOrder, OrderAggregate> Deliver = (order, command) =>
        {
            command.Metadata.CorrelationId = order.ProcessId;
            order.Deliver(command);
            return order;
        };
    }
}
