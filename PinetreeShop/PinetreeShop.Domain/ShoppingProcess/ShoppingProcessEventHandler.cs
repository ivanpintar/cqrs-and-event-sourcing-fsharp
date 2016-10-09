using System;
using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.Domain.Baskets.Events;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.Domain.Products.Events;

namespace PinetreeShop.Domain.ShoppingProcess
{
    public class ShoppingProcessEventHandler :
        IHandleEvent<BasketCreated>,
        IHandleEvent<BasketAddItemTried>,
        IHandleEvent<ProductReserved>,
        IHandleEvent<ProductReservationFailed>,
        IHandleEvent<BasketItemRemoved>,
        IHandleEvent<BasketCancelled>,
        IHandleEvent<BasketCheckedOut>
    {
        private IProcessManagerRepository _processManagerRepository;

        public ShoppingProcessEventHandler(IProcessManagerRepository processManagerRepository)
        {
            _processManagerRepository = processManagerRepository;
        }

        public IProcessManager Handle(BasketCheckedOut evt)
        {
            throw new NotImplementedException();
        }

        public IProcessManager Handle(BasketItemRemoved evt)
        {
            throw new NotImplementedException();
        }

        public IProcessManager Handle(BasketCancelled evt)
        {
            throw new NotImplementedException();
        }

        public IProcessManager Handle(BasketCreated evt)
        {
            var shoppingProcessManager = _processManagerRepository.GetProcessManagerById<ShoppingProcessManager>(evt.Metadata.CorrelationId);
            shoppingProcessManager.BasketCreated(evt);
            return shoppingProcessManager;
        }

        public IProcessManager Handle(BasketAddItemTried evt)
        {
            var shoppingProcessManager = _processManagerRepository.GetProcessManagerById<ShoppingProcessManager>(evt.Metadata.CorrelationId);
            shoppingProcessManager.BasketAddItemTried(evt);
            return shoppingProcessManager;
        }

        public IProcessManager Handle(ProductReserved evt)
        {
            var shoppingProcessManager = _processManagerRepository.GetProcessManagerById<ShoppingProcessManager>(evt.Metadata.CorrelationId);
            shoppingProcessManager.ProductReserved(evt);
            return shoppingProcessManager;
        }

        public IProcessManager Handle(ProductReservationFailed evt)
        {
            var shoppingProcessManager = _processManagerRepository.GetProcessManagerById<ShoppingProcessManager>(evt.Metadata.CorrelationId);
            shoppingProcessManager.ProductReservationFailed(evt);
            return shoppingProcessManager;
        }
    }
}
