using System;
using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.Domain.Baskets.Events;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.Domain.Products.Events;

namespace PinetreeShop.Domain.OrderProcess
{
    public class OrderProcessEventHandler :
        IHandleEvent<BasketCheckedOut>,
        IHandleEvent<ProductReserved>
    {
        private IProcessManagerRepository _processManagerRepository;

        public OrderProcessEventHandler(IProcessManagerRepository processManagerRepository)
        {
            _processManagerRepository = processManagerRepository;
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
