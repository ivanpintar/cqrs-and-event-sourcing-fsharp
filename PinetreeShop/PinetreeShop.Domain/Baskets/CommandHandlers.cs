using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.Domain.Baskets.Commands;
using System;

namespace PinetreeShop.Domain.Baskets
{
    public class BasketCommandHandler :
        IHandle<CreateBasket>,
        IHandle<AddProduct>,
        IHandle<RemoveProduct>,
        IHandle<Cancel>,
        IHandle<Checkout>
    {
        private IDomainRepository _domainRepository;

        public BasketCommandHandler(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        public IAggregate Handle(AddProduct command)
        {
            throw new NotImplementedException();
        }

        public IAggregate Handle(CreateBasket command)
        {
            throw new NotImplementedException();
        }

        public IAggregate Handle(RemoveProduct command)
        {
            throw new NotImplementedException();
        }

        public IAggregate Handle(Cancel command)
        {
            throw new NotImplementedException();
        }

        public IAggregate Handle(Checkout command)
        {
            throw new NotImplementedException();
        }
    }
}
