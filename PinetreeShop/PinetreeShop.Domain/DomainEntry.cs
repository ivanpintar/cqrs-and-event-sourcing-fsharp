using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.CommandsAndEvents;
using PinetreeShop.CQRS.Infrastructure.Repositories;
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
        private readonly WorkflowEventListener _workflowEventListener;

        public DomainEntry(
            IAggregateRepository aggregateRepository,
            IWorkflowRepository workflowRepository)
        {
            _commandDispatcher = CreateCommandDispatcher(aggregateRepository);
            _workflowEventListener = CreateEventListener(workflowRepository, _commandDispatcher);
        }

        public void ExecuteCommand<TCommand>(TCommand command) where TCommand : ICommand
        {
            _commandDispatcher.ExecuteCommand(command);
        }

        public void HandleEvent<TEvent>(TEvent evt) where TEvent : IEvent
        {
            _workflowEventListener.HandleEvent(evt);
        }

        private WorkflowEventListener CreateEventListener(IWorkflowRepository workflowRepository, CommandDispatcher commandDispatcher)
        {
            var eventListener = new WorkflowEventListener(workflowRepository, commandDispatcher);

            return eventListener;
        }

        private CommandDispatcher CreateCommandDispatcher(IAggregateRepository aggregateRepository)
        {
            var commandDispatcher = new CommandDispatcher(aggregateRepository);

            var productCommandHandler = new ProductCommandHandler(aggregateRepository);
            commandDispatcher.RegisterHandler<CreateProduct>(productCommandHandler);
            commandDispatcher.RegisterHandler<ChangeProductQuantity>(productCommandHandler);
            commandDispatcher.RegisterHandler<ReserveProduct>(productCommandHandler);
            commandDispatcher.RegisterHandler<ReleaseProductReservation>(productCommandHandler);

            var basketCommandHandler = new BasketCommandHandler(aggregateRepository);
            commandDispatcher.RegisterHandler<CreateBasket>(basketCommandHandler);
            commandDispatcher.RegisterHandler<AddProduct>(basketCommandHandler);
            commandDispatcher.RegisterHandler<RevertAddProduct>(basketCommandHandler);
            commandDispatcher.RegisterHandler<RemoveProduct>(basketCommandHandler);
            commandDispatcher.RegisterHandler<Cancel>(basketCommandHandler);
            commandDispatcher.RegisterHandler<CheckOut>(basketCommandHandler);
            commandDispatcher.RegisterHandler<RevertCheckOut>(basketCommandHandler);

            var orderCommandHandler = new OrderCommandHandler(aggregateRepository);
            commandDispatcher.RegisterHandler<CreateOrder>(orderCommandHandler);
            commandDispatcher.RegisterHandler<CancelOrder>(orderCommandHandler);
            commandDispatcher.RegisterHandler<ShipOrder>(orderCommandHandler);
            commandDispatcher.RegisterHandler<DeliverOrder>(orderCommandHandler);

            return commandDispatcher;
        }
    }
}
