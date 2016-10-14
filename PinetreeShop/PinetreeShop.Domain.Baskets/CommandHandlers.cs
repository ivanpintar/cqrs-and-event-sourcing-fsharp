using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.CQRS.Persistence.Exceptions;
using PinetreeShop.Domain.Baskets.Commands;
using PinetreeShop.Domain.Shared.Exceptions;
using System;

namespace PinetreeShop.Domain.Baskets
{
    public static class CommandHandler
    {
        public static Func<BasketAggregate, AddItemToBasket, BasketAggregate> AddItem = (basket, command) =>
        {
            basket.AddItemToBasket(command);
            return basket;
        };

        public static Func<BasketAggregate, CreateBasket, BasketAggregate> Create = (basket, command) =>
        {
            if (basket != null)
            {
                throw new AggregateExistsException(command.AggregateId, "Order already exists");
            }

            return BasketAggregate.Create(command);
        };

        public static Func<BasketAggregate, RemoveItemFromBasket, BasketAggregate> RemoveItem = (basket, command) =>
        {
            basket.RemoveItemFromBasket(command);
            return basket;
        };

        public static Func<BasketAggregate, CancelBasket, BasketAggregate> Cancel = (basket, command) =>
        {
            basket.Cancel(command);
            return basket;
        };

        public static Func<BasketAggregate, CheckOutBasket, BasketAggregate> CheckOut = (basket, command) =>
        {
            basket.CheckOut(command);
            return basket;
        };
    }
}
