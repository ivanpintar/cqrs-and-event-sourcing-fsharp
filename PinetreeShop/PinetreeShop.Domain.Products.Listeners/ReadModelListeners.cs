using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.CQRS.Persistence.SQL;
using PinetreeShop.Domain.Products.Events;
using PinetreeShop.Domain.Products.ReadModel;
using PinetreeShop.Domain.Products.ReadModel.Entities;
using System.Linq;
using System.Transactions;

namespace PinetreeShop.Domain.Products.Listeners
{
    public class ReadModelListeners
    {
        private EventStreamListener _eventStreamListener;
        private ProductContext _ctx;

        public ReadModelListeners()
        {
            var eventStore = new SqlEventStore();
            _eventStreamListener = new EventStreamListener(eventStore);

            _eventStreamListener.RegisterEventHandler<ProductCreated>(OnProductCreated);
            _eventStreamListener.RegisterEventHandler<ProductQuantityChanged>(OnProductQuantityChanged);
            _eventStreamListener.RegisterEventHandler<ProductReserved>(OnProductReserved);
            _eventStreamListener.RegisterEventHandler<ProductReservationCancelled>(OnProductReservationCancelled);
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
            using (var transaction = new TransactionScope())
            using (_ctx = new ProductContext())
            {
                int lastEventNumber = GetLastEventNumber();

                _eventStreamListener.ReadAndHandleLatestEvents<ProductAggregate>(lastEventNumber);
                _ctx.SaveChanges();

                transaction.Complete();
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
