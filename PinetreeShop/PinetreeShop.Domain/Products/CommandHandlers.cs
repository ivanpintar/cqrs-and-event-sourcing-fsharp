using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.Domain.Products.Commands;
using System;

namespace PinetreeShop.Domain.Products.CommandHandlers
{
    public class ProductCommandHandler :
        IHandle<CreateProduct>,
        IHandle<ChangeProductQuantity>,
        IHandle<ReserveProduct>,
        IHandle<ReleaseProductReservation>
    {
        private IDomainRepository _domainRepository;

        public ProductCommandHandler(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        public IAggregate Handle(ChangeProductQuantity command)
        {
            throw new NotImplementedException();
        }

        public IAggregate Handle(CreateProduct command)
        {
            throw new NotImplementedException();
        }

        public IAggregate Handle(ReserveProduct command)
        {
            throw new NotImplementedException();
        }

        public IAggregate Handle(ReleaseProductReservation command)
        {
            throw new NotImplementedException();
        }
    }
}
