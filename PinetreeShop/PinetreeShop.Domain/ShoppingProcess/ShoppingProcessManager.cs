using PinetreeShop.CQRS.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PinetreeShop.Domain.Baskets.Events;
using PinetreeShop.Domain.Products.Commands;
using PinetreeShop.Domain.Products.Events;
using PinetreeShop.Domain.Baskets.Commands;

namespace PinetreeShop.Domain.ShoppingProcess
{
    public class ShoppingProcessManager : ProcessManagerBase
    {
        private Guid _basketId;

        public ShoppingProcessManager()
        {
            RegisterEventHandler<BasketCreated>(Apply);
            RegisterEventHandler<BasketAddItemTried>(Apply);
            RegisterEventHandler<ProductReserved>(Apply);            
            RegisterEventHandler<ProductReservationFailed>(Apply);
            RegisterEventHandler<BasketItemRemoved>(Apply);
            RegisterEventHandler<BasketCheckedOut>(Apply);
            RegisterEventHandler<BasketCancelled>(Apply);
        }

        private void Apply(BasketCancelled obj)
        {
            throw new NotImplementedException();
        }

        private void Apply(BasketCheckedOut obj)
        {
            throw new NotImplementedException();
        }

        private void Apply(BasketItemRemoved obj)
        {
            throw new NotImplementedException();
        }

        private void Apply(ProductReservationFailed evt)
        {
            DispatchCommand(new RevertAddItemToBasket(_basketId, evt.AggregateId, evt.Quantity, evt.Reason));
        }

        private void Apply(ProductReserved evt)
        {
            DispatchCommand(new ConfirmAddItemToBasket(_basketId, evt.AggregateId, evt.QuantityToReserve));
        }

        private void Apply(BasketAddItemTried evt)
        {
            DispatchCommand(new ReserveProduct(evt.ProductId, _basketId, evt.Quantity));
        }

        private void Apply(BasketCreated evt)
        {
            ProcessId = evt.Metadata.CorrelationId;
            _basketId = evt.AggregateId;
        }

        internal void BasketCreated(BasketCreated evt)
        {
            HandleEvent(evt);
        }

        internal void BasketAddItemTried(BasketAddItemTried evt)
        {
            HandleEvent(evt);
        }

        internal void ProductReserved(ProductReserved evt)
        {
            HandleEvent(evt);
        }

        internal void ProductReservationFailed(ProductReservationFailed evt)
        {
            HandleEvent(evt);
        }
    }
}
