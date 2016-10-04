using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.CommandsAndEvents;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.CQRS.Persistence.Exceptions;
using PinetreeShop.Domain.Baskets.Commands;
using PinetreeShop.Domain.Exceptions;
using System;

namespace PinetreeShop.Domain.Baskets
{
    public class BasketCommandHandler :
        IHandleCommand<CreateBasket>,
        IHandleCommand<AddProduct>,
        IHandleCommand<RevertAddProduct>,
        IHandleCommand<RemoveProduct>,
        IHandleCommand<Cancel>,
        IHandleCommand<CheckOut>,
        IHandleCommand<RevertCheckOut>
    {
        private IAggregateRepository _aggregateRepository;

        public BasketCommandHandler(IAggregateRepository aggregateRepository)
        {
            _aggregateRepository = aggregateRepository;
        }

        public IAggregate Handle(AddProduct command)
        {
            var basket = _aggregateRepository.GetAggregateById<Basket>(command.AggregateId);
            basket.AddProduct(command.AggregateId, command.ProductId, command.ProductName, command.Price, command.Quantity);
            return basket;
        }

        public IAggregate Handle(RevertCheckOut command)
        {
            var basket = _aggregateRepository.GetAggregateById<Basket>(command.AggregateId);
            basket.RevertCheckout(command.AggregateId, command.Reason);
            return basket;
        }

        public IAggregate Handle(RevertAddProduct command)
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

        public IAggregate Handle(RemoveProduct command)
        {
            var basket = _aggregateRepository.GetAggregateById<Basket>(command.AggregateId);
            basket.RemoveProduct(command.AggregateId, command.ProductId, command.Quantity);
            return basket;
        }

        public IAggregate Handle(Cancel command)
        {
            var basket = _aggregateRepository.GetAggregateById<Basket>(command.AggregateId);
            basket.Cancel(command.AggregateId);
            return basket;
        }

        public IAggregate Handle(CheckOut command)
        {
            var basket = _aggregateRepository.GetAggregateById<Basket>(command.AggregateId);
            basket.CheckOut(command.AggregateId, command.ShippingAddress);
            return basket;
        }
    }
}
