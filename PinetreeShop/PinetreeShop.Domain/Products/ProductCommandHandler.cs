using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.CQRS.Persistence.Exceptions;
using PinetreeShop.Domain.Exceptions;
using PinetreeShop.Domain.Products.Commands;

namespace PinetreeShop.Domain.Products
{
    public class ProductCommandHandler :
        IHandleCommand<CreateProduct>,
        IHandleCommand<ChangeProductQuantity>,
        IHandleCommand<ReserveProduct>,
        IHandleCommand<CancelProductReservation>
    {
        private IAggregateRepository _aggregateRepository;

        public ProductCommandHandler(IAggregateRepository aggregateRepository)
        {
            _aggregateRepository = aggregateRepository;
        }

        public IAggregate Handle(CreateProduct command)
        {
            try
            {
                var product = _aggregateRepository.GetAggregateById<Product>(command.AggregateId);
                throw new AggregateExistsException(command.AggregateId, "Product already exists");
            }
            catch (AggregateNotFoundException)
            {
                // We expect not to find anything
            }
            return Product.Create(command.AggregateId, command.Name, command.Price);
        }

        public IAggregate Handle(ChangeProductQuantity command)
        {
            var product = _aggregateRepository.GetAggregateById<Product>(command.AggregateId);
            product.ChangeQuantity(command.AggregateId, command.Difference);
            return product;
        }

        public IAggregate Handle(ReserveProduct command)
        {
            var product = _aggregateRepository.GetAggregateById<Product>(command.AggregateId);
            product.Reserve(command.AggregateId, command.BasketId, command.Quantity);
            return product;
        }

        public IAggregate Handle(CancelProductReservation command)
        {
            var product = _aggregateRepository.GetAggregateById<Product>(command.AggregateId);
            product.CancelReservation(command.AggregateId, command.Quantity);
            return product;
        }
    }
}
