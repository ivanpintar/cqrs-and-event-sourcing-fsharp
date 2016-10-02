using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Persistence.Exceptions;
using PinetreeShop.Domain.Products.Commands;
using PinetreeShop.Domain.Products.Exceptions;
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

        public IAggregate Handle(CreateProduct command)
        {
            try
            {
                var product = _domainRepository.GetById<Product>(command.AggregateId);
                throw new ProductExistsException(command.AggregateId, "Product already exists");
            }
            catch (AggregateNotFoundException)
            {
                // We expect not to find anything
            }
            return Product.Create(command.AggregateId, command.Name, command.Price);
        }

        public IAggregate Handle(ChangeProductQuantity command)
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
