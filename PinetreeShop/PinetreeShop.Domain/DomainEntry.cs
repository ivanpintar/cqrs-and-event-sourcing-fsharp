using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.Domain.Baskets;
using PinetreeShop.Domain.Baskets.Commands;
using PinetreeShop.Domain.Orders;
using PinetreeShop.Domain.Orders.Commands;
using PinetreeShop.Domain.Products;
using PinetreeShop.Domain.Products.Commands;
using PinetreeShop.Domain.ShopProcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinetreeShop.Domain
{
    public class DomainEntry
    {
        private readonly CommandDispatcher _commandDispatcher;

        public DomainEntry(
            IAggregateRepository aggregateRepository)
        {
            _commandDispatcher = CreateCommandDispatcher(aggregateRepository);
        }

        public void ExecuteCommand<TCommand>(TCommand command) where TCommand : ICommand
        {
            _commandDispatcher.ExecuteCommand(command);
        }

        private CommandDispatcher CreateCommandDispatcher(IAggregateRepository aggregateRepository)
        {
            var commandDispatcher = new CommandDispatcher(aggregateRepository);

            var productCommandHandler = new ProductCommandHandler(aggregateRepository);
            commandDispatcher.RegisterHandler<CreateProduct>(productCommandHandler);
            commandDispatcher.RegisterHandler<ChangeProductQuantity>(productCommandHandler);
            commandDispatcher.RegisterHandler<ReserveProduct>(productCommandHandler);
            commandDispatcher.RegisterHandler<CancelProductReservation>(productCommandHandler);

            var basketCommandHandler = new BasketCommandHandler(aggregateRepository);
            commandDispatcher.RegisterHandler<CreateBasket>(basketCommandHandler);
            commandDispatcher.RegisterHandler<TryAddItemToBasket>(basketCommandHandler); 
            commandDispatcher.RegisterHandler<ConfirmAddItemToBasket>(basketCommandHandler);
            commandDispatcher.RegisterHandler<RevertAddItemToBasket>(basketCommandHandler);
            commandDispatcher.RegisterHandler<RemoveItemFromBasket>(basketCommandHandler);
            commandDispatcher.RegisterHandler<CancelBasket>(basketCommandHandler);
            commandDispatcher.RegisterHandler<CheckOutBasket>(basketCommandHandler);

            var orderCommandHandler = new OrderCommandHandler(aggregateRepository);
            commandDispatcher.RegisterHandler<CreateOrder>(orderCommandHandler);
            commandDispatcher.RegisterHandler<CancelOrder>(orderCommandHandler);
            commandDispatcher.RegisterHandler<ShipOrder>(orderCommandHandler);
            commandDispatcher.RegisterHandler<DeliverOrder>(orderCommandHandler);

            return commandDispatcher;
        }
    }
}
