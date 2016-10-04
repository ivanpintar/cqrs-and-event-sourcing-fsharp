using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.CommandsAndEvents;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.CQRS.Persistence.Exceptions;
using PinetreeShop.Domain.Exceptions;
using PinetreeShop.Domain.Orders.Commands;
using System;

namespace PinetreeShop.Domain.Orders
{
    public class OrderCommandHandler :
        IHandleCommand<CreateOrder>,
        IHandleCommand<CancelOrder>,
        IHandleCommand<ShipOrder>,
        IHandleCommand<DeliverOrder>
    {
        private IAggregateRepository _aggregateRepository;

        public OrderCommandHandler(IAggregateRepository aggregateRepository)
        {
            _aggregateRepository = aggregateRepository;
        }

        public IAggregate Handle(CreateOrder command)
        {
            try
            {
                var order = _aggregateRepository.GetAggregateById<Order>(command.AggregateId);
                throw new AggregateExistsException(command.AggregateId, "Order already exists");
            }
            catch (AggregateNotFoundException)
            {
                // We expect not to find anything
            }
            return Order.Create(command.AggregateId, command.BasketId, command.Lines, command.ShippingAddress);
        }

        public IAggregate Handle(CancelOrder command)
        {
            var order = _aggregateRepository.GetAggregateById<Order>(command.AggregateId);
            order.Cancel(command.AggregateId);
            return order;
        }

        public IAggregate Handle(ShipOrder command)
        {
            var order = _aggregateRepository.GetAggregateById<Order>(command.AggregateId);
            order.Ship(command.AggregateId);
            return order;
        }

        public IAggregate Handle(DeliverOrder command)
        {

            var order = _aggregateRepository.GetAggregateById<Order>(command.AggregateId);
            order.Deliver(command.AggregateId);
            return order;
        }
    }
}
