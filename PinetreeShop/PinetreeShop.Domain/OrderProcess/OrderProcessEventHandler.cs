using System;
using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.Domain.Baskets.Events;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.Domain.Products.Events;
using PinetreeShop.Domain.Orders.Events;

namespace PinetreeShop.Domain.OrderProcess
{
    public class OrderProcessEventHandler :
        IHandleEvent<BasketCheckedOut>,
        IHandleEvent<OrderCreated>,
        IHandleEvent<CreateOrderFailed>,
        IHandleEvent<ProductReserved>,
        IHandleEvent<ProductReservationFailed>,
        IHandleEvent<OrderCancelled>,
        IHandleEvent<OrderShipped>,
        IHandleEvent<OrderDelivered>
    {
        private IProcessManagerRepository _processManagerRepository;

        public OrderProcessEventHandler(IProcessManagerRepository processManagerRepository)
        {
            _processManagerRepository = processManagerRepository;
        }

        public IProcessManager Handle(CreateOrderFailed evt)
        {
            var process = _processManagerRepository.GetProcessManagerById<OrderProcessManager>(evt.Metadata.CorrelationId);
            process.CreateOrderFailed(evt);
            return process;
        }

        public IProcessManager Handle(OrderShipped evt)
        {
            var process = _processManagerRepository.GetProcessManagerById<OrderProcessManager>(evt.Metadata.CorrelationId);
            process.OrderShipped(evt);
            return process;
        }

        public IProcessManager Handle(OrderDelivered evt)
        {
            var process = _processManagerRepository.GetProcessManagerById<OrderProcessManager>(evt.Metadata.CorrelationId);
            process.OrderDelivered(evt);
            return process;
        }

        public IProcessManager Handle(OrderCancelled evt)
        {
            var process = _processManagerRepository.GetProcessManagerById<OrderProcessManager>(evt.Metadata.CorrelationId);
            process.OrderCancelled(evt);
            return process;
        }

        public IProcessManager Handle(OrderCreated evt)
        {
            var process = _processManagerRepository.GetProcessManagerById<OrderProcessManager>(evt.Metadata.CorrelationId);
            process.OrderCreated(evt);
            return process;
        }

        public IProcessManager Handle(ProductReservationFailed evt)
        {
            var process = _processManagerRepository.GetProcessManagerById<OrderProcessManager>(evt.Metadata.CorrelationId);
            process.ProductReservationFailed(evt);
            return process;
        }

        public IProcessManager Handle(BasketCheckedOut evt)
        {
            var process = _processManagerRepository.GetProcessManagerById<OrderProcessManager>(evt.Metadata.CorrelationId);
            process.BasketCheckedOut(evt);
            return process;
        }

        public IProcessManager Handle(ProductReserved evt)
        {
            var process = _processManagerRepository.GetProcessManagerById<OrderProcessManager>(evt.Metadata.CorrelationId);
            process.ProductReserved(evt);
            return process;
        }


    }
}
