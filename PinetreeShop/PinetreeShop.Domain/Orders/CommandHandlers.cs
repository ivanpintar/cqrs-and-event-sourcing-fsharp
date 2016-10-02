using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.Domain.Orders.Commands;
using System;

namespace PinetreeShop.Domain.Orders
{
    public class OrderCommandHandler :
        IHandle<CreateOrder>,
        IHandle<CancelOrder>,
        IHandle<ShipOrder>,
        IHandle<DeliverOrder>
    {
        private IDomainRepository _domainRepository;

        public OrderCommandHandler(IDomainRepository domainRepository)
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
