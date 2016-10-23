using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.Domain.Baskets.Commands;
using PinetreeShop.Domain.Shared.Exceptions;
using System;
using PinetreeShop.CQRS.Infrastructure;

namespace PinetreeShop.Domain.Baskets
{
    public class BasketCommandDispatcher : CommandDispatcher
    {
        public BasketCommandDispatcher(IAggregateRepository aggregateRepository) : base(aggregateRepository)
        {
            RegisterHandler(Create);
            RegisterHandler(RemoveItem);
            RegisterHandler(CheckOut);
            RegisterHandler(AddItem);
            RegisterHandler(Cancel);
        }

        private Func<BasketAggregate, AddItemToBasket, BasketAggregate> AddItem = (basket, command) =>
        {
            basket.AddItemToBasket(command);
            return basket;
        };

        private Func<BasketAggregate, CreateBasket, BasketAggregate> Create = (basket, command) =>
        {
            if (basket != null)
            {
                throw new AggregateExistsException(command.AggregateId, "Order already exists");
            }

            return BasketAggregate.Create(command);
        };

        private Func<BasketAggregate, RemoveItemFromBasket, BasketAggregate> RemoveItem = (basket, command) =>
        {
            basket.RemoveItemFromBasket(command);
            return basket;
        };

        private Func<BasketAggregate, CancelBasket, BasketAggregate> Cancel = (basket, command) =>
        {
            basket.Cancel(command);
            return basket;
        };

        private Func<BasketAggregate, CheckOutBasket, BasketAggregate> CheckOut = (basket, command) =>
        {
            basket.CheckOut(command);
            return basket;
        };        
    }
}
