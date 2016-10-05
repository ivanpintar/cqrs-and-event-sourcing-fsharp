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
            var basket = _aggregateRepository.GetAggregateById<Basket>(command.AggregateId);
            basket.TryAddProduct(command.AggregateId, command.ProductId, command.ProductName, command.Price, command.Quantity);
            return basket;
        }

        public IAggregate Handle(ConfirmAddItemToBasket command)
        {
            var basket = _aggregateRepository.GetAggregateById<Basket>(command.AggregateId);
            basket.ConfirmAddItem(command.AggregateId, command.ProductId, command.Quantity);
            return basket;
        }

        public IAggregate Handle(RevertAddItemToBasket command)
        {
            var basket = _aggregateRepository.GetAggregateById<Basket>(command.AggregateId);
            basket.RevertAddProduct(command.AggregateId, command.ProductId, command.Quantity, command.Reason);
            return basket;
        }

        public IAggregate Handle(CreateBasket command)
        {
            try
            {
                var basket = _aggregateRepository.GetAggregateById<Basket>(command.AggregateId);
                throw new AggregateExistsException(command.AggregateId, "Order already exists");
            }
            catch (AggregateNotFoundException)
            {
                // We expect not to find anything
            }
            return Basket.Create(command.AggregateId);
        }

        public IAggregate Handle(RemoveItemFromBasket command)
        {
            var basket = _aggregateRepository.GetAggregateById<Basket>(command.AggregateId);
            basket.RemoveProduct(command.AggregateId, command.ProductId, command.Quantity);
            return basket;
        }

        public IAggregate Handle(CancelBasket command)
        {
            var basket = _aggregateRepository.GetAggregateById<Basket>(command.AggregateId);
            basket.Cancel(command.AggregateId);
            return basket;
        }

        public IAggregate Handle(CheckOutBasket command)
        {
            var basket = _aggregateRepository.GetAggregateById<Basket>(command.AggregateId);
            basket.TryCheckOut(command.AggregateId, command.ShippingAddress);
            return basket;
        }
    }
}
