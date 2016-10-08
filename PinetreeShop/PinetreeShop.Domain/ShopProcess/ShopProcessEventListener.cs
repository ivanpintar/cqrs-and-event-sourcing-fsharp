using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.Domain.Baskets.Events;
using PinetreeShop.Domain.Products.Events;
using System;
using PinetreeShop.CQRS.Infrastructure.Commands;

namespace PinetreeShop.Domain.ShopProcess
{
    public class ShopProcessEventListener :
        IHandleEvent<BasketCreated>,
        IHandleEvent<BasketAddItemTried>,
        IHandleEvent<ProductReserved>,
        IHandleEvent<ProductReservationFailed>,
        IHandleEvent<BasketItemRemoved>,
        IHandleEvent<BasketCheckedOut>
    {
        private CommandDispatcher _commandDispatcher;

        public ShopProcessEventListener(CommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        public void Handle(BasketCheckedOut evt)
        {
            throw new NotImplementedException();
        }

        public void Handle(BasketAddItemTried evt)
        {
            throw new NotImplementedException();
        }

        public void Handle(BasketCreated evt)
        {
            throw new NotImplementedException();
        }

        public void Handle(ProductReserved evt)
        {
            throw new NotImplementedException();
        }

        public void Handle(BasketItemRemoved evt)
        {
            throw new NotImplementedException();
        }
        
        public void Handle(ProductReservationFailed evt)
        {
            throw new NotImplementedException();
        }
    }
}
