using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.CommandsAndEvents;
using PinetreeShop.CQRS.Infrastructure.Repositories;
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
        private IAggregateRepository _domainRepository;

        public OrderCommandHandler(IAggregateRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        public IAggregate Handle(CreateOrder command)
        {
            throw new NotImplementedException();
        }

        public IAggregate Handle(CancelOrder command)
        {
            throw new NotImplementedException();
        }

        public IAggregate Handle(ShipOrder command)
        {
            throw new NotImplementedException();
        }

        public IAggregate Handle(DeliverOrder command)
        {
            throw new NotImplementedException();
        }
    }
}
