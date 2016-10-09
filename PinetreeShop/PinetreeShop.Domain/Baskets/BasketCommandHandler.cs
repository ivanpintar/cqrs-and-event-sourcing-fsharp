using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.CQRS.Persistence.Exceptions;
using PinetreeShop.Domain.Baskets.Commands;
using PinetreeShop.Domain.Exceptions;
using System;

namespace PinetreeShop.Domain.Baskets
{
    public class BasketCommandHandler :
        IHandleCommand<CreateBasket>,
        IHandleCommand<TryAddItemToBasket>,
        IHandleCommand<ConfirmAddItemToBasket>,
        IHandleCommand<RevertAddItemToBasket>,
        IHandleCommand<RemoveItemFromBasket>,
        IHandleCommand<CancelBasket>,
        IHandleCommand<CheckOutBasket>
    {
        private IAggregateRepository _aggregateRepository;

        public BasketCommandHandler(IAggregateRepository aggregateRepository)
        {
            _aggregateRepository = aggregateRepository;
        }

        public IAggregate Handle(TryAddItemToBasket command)
        {
            var basket = _aggregateRepository.GetAggregateById<BasketAggregate>(command.AggregateId);
            basket.TryAddItemToBasket(command);
            return basket;
        }

        public IAggregate Handle(ConfirmAddItemToBasket command)
        {
            var basket = _aggregateRepository.GetAggregateById<BasketAggregate>(command.AggregateId);
            basket.ConfirmAddItem(command);
            return basket;
        }

        public IAggregate Handle(RevertAddItemToBasket command)
        {
            var basket = _aggregateRepository.GetAggregateById<BasketAggregate>(command.AggregateId);
            basket.RevertAddProduct(command);
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
