using PinetreeCQRS.Infrastructure.Events;
using PinetreeCQRS.Persistence.SQL;
using PinetreeShop.Domain.Products.Events;
using PinetreeShop.Domain.Products.ReadModel;
using PinetreeShop.Domain.Products.ReadModel.Entities;
using System.Linq;
using System.Transactions;
using System;

namespace PinetreeShop.Domain.Products.Listeners
{
    public class ReadModelListener
    {
        private EventStreamListener _eventStreamListener;
        private ProductContext _ctx;

        public ReadModelListener()
        {
            var eventStore = new SqlEventStore();
            _eventStreamListener = new EventStreamListener(eventStore);

            _eventStreamListener.RegisterEventHandler<ProductCreated>(OnProductCreated);
            _eventStreamListener.RegisterEventHandler<ProductQuantityChanged>(OnProductQuantityChanged);
            _eventStreamListener.RegisterEventHandler<ProductReserved>(OnProductReserved);
            _eventStreamListener.RegisterEventHandler<ProductReservationCancelled>(OnProductReservationCancelled);
            _eventStreamListener.RegisterEventHandler<ReservedProductPurchased>(OnResrvedProductPurchased);
        }

        private void OnResrvedProductPurchased(ReservedProductPurchased evt)
        {
            var product = GetProduct(evt);
            product.Reserved -= evt.Quantity;
            product.Quantity -= evt.Quantity;
            product.LastEventNumber = evt.Metadata.EventNumber;
            _ctx.SaveChanges();
        }

        private void OnProductReservationCancelled(ProductReservationCancelled evt)
        {
            var product = GetProduct(evt);
            product.Reserved -= evt.Quantity;
            product.LastEventNumber = evt.Metadata.EventNumber;
            _ctx.SaveChanges();
        }

        private void OnProductReserved(ProductReserved evt)
        {
            var product = GetProduct(evt);
            product.Reserved += evt.QuantityToReserve;
            product.LastEventNumber = evt.Metadata.EventNumber;
            _ctx.SaveChanges();
        }

        private void OnProductQuantityChanged(ProductQuantityChanged evt)
        {
            var product = GetProduct(evt);
            product.Quantity += evt.Difference;
            product.LastEventNumber = evt.Metadata.EventNumber;
            _ctx.SaveChanges();
        }

        private void OnProductCreated(ProductCreated evt)
        {
            _ctx.Products.Add(new Product
            {
                Id = evt.AggregateId,
                Name = evt.Name,
                Price = evt.Price,
                Quantity = 0,
                Reserved = 0,
                LastEventNumber = evt.Metadata.EventNumber
            });
            _ctx.SaveChanges();
        }

        public void ProcessEvents()
        {
            using (_ctx = new ProductContext())
            {
                int lastEventNumber = GetLastEventNumber();
                _eventStreamListener.ReadAndHandleLatestEvents<ProductAggregate>(lastEventNumber);
            }
        }

        private int GetLastEventNumber()
        {
            if (!_ctx.Products.Any()) return 0;
            return _ctx.Products.Max(p => p.LastEventNumber);
        }

        private Product GetProduct(IEvent evt)
        {
            return _ctx.Products.Single(p => p.Id == evt.AggregateId);
        }

    }
}
