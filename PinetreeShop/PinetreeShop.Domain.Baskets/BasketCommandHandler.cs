using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.CQRS.Persistence.Exceptions;
using PinetreeShop.Domain.Baskets.Commands;
using PinetreeShop.Domain.Shared.Exceptions;

namespace PinetreeShop.Domain.Baskets
{
    public class BasketCommandHandler :
        IHandleCommand<CreateBasket>,
        IHandleCommand<AddItemToBasket>,
        IHandleCommand<RemoveItemFromBasket>,
        IHandleCommand<CancelBasket>,
        IHandleCommand<CheckOutBasket>
    {
        private IAggregateRepository _aggregateRepository;

        public BasketCommandHandler(IAggregateRepository aggregateRepository)
        {
            _aggregateRepository = aggregateRepository;
        }

        public IAggregate Handle(AddItemToBasket command)
        {
            var basket = _aggregateRepository.GetAggregateById<BasketAggregate>(command.AggregateId);
            basket.AddItemToBasket(command);
            return basket;
        }

        public IAggregate Handle(CreateBasket command)
        {
            try
            {
                var basket = _aggregateRepository.GetAggregateById<BasketAggregate>(command.AggregateId);
                throw new AggregateExistsException(command.AggregateId, "Order already exists");
            }
            catch (AggregateNotFoundException)
            {
                // We expect not to find anything
            }
            return BasketAggregate.Create(command);
        }

        public IAggregate Handle(RemoveItemFromBasket command)
        {
            var basket = _aggregateRepository.GetAggregateById<BasketAggregate>(command.AggregateId);
            basket.RemoveItemFromBasket(command);
            return basket;
        }

        public IAggregate Handle(CancelBasket command)
        {
            var basket = _aggregateRepository.GetAggregateById<BasketAggregate>(command.AggregateId);
            basket.Cancel(command);
            return basket;
        }

        public IAggregate Handle(CheckOutBasket command)
        {
            var basket = _aggregateRepository.GetAggregateById<BasketAggregate>(command.AggregateId);
            basket.CheckOut(command);
            return basket;
        }
    }
}
