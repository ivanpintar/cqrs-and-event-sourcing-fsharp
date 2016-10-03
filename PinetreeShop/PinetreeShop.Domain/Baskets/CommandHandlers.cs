using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.CommandsAndEvents;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.Domain.Baskets.Commands;
using System;

namespace PinetreeShop.Domain.Baskets
{
    public class BasketCommandHandler :
        IHandleCommand<CreateBasket>,
        IHandleCommand<AddProduct>,
        IHandleCommand<RemoveProduct>,
        IHandleCommand<Cancel>,
        IHandleCommand<Checkout>
    {
        private IAggregateRepository _domainRepository;

        public BasketCommandHandler(IAggregateRepository domainRepository)
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
