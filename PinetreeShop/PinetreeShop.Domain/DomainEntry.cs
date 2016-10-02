using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.Domain.Baskets;
using PinetreeShop.Domain.Baskets.Commands;
using PinetreeShop.Domain.Orders;
using PinetreeShop.Domain.Orders.Commands;
using PinetreeShop.Domain.Products.CommandHandlers;
using PinetreeShop.Domain.Products.Commands;
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
            IDomainRepository domainRepository,
            IEnumerable<Action<ICommand>> preExecutionPipe = null,
            IEnumerable<Action<object>> postExecutionPipe = null) 
        {
            _commandDispatcher = CreateCommandDispatcher(domainRepository, preExecutionPipe, postExecutionPipe);
        }

        public void ExecuteCommand<TCommand>(TCommand command) where TCommand : ICommand
        {
            _commandDispatcher.ExecuteCommand(command);
        }

        private CommandDispatcher CreateCommandDispatcher(IDomainRepository domainRepository, IEnumerable<Action<ICommand>> preExecutionPipe, IEnumerable<Action<object>> postExecutionPipe)
        {
            var commandDispatcher = new CommandDispatcher(domainRepository, preExecutionPipe, postExecutionPipe);

            var productCommandHandler = new ProductCommandHandler(domainRepository);
            commandDispatcher.RegisterHander<CreateProduct>(productCommandHandler);
            commandDispatcher.RegisterHander<ChangeProductQuantity>(productCommandHandler);
            commandDispatcher.RegisterHander<ReserveProduct>(productCommandHandler);
            commandDispatcher.RegisterHander<ReleaseProductReservation>(productCommandHandler);

            var basketCommandHandler = new BasketCommandHandler(domainRepository);
            commandDispatcher.RegisterHander<CreateBasket>(basketCommandHandler);
            commandDispatcher.RegisterHander<AddProduct>(basketCommandHandler);
            commandDispatcher.RegisterHander<RemoveProduct>(basketCommandHandler);
            commandDispatcher.RegisterHander<Checkout>(basketCommandHandler);

            var orderCommandHandler = new OrderCommandHandler(domainRepository);
            commandDispatcher.RegisterHander<CreateOrder>(orderCommandHandler);
            commandDispatcher.RegisterHander<CancelOrder>(orderCommandHandler);
            commandDispatcher.RegisterHander<ShipOrder>(orderCommandHandler);
            commandDispatcher.RegisterHander<DeliverOrder>(orderCommandHandler);

            return commandDispatcher;
        }
    }
}
