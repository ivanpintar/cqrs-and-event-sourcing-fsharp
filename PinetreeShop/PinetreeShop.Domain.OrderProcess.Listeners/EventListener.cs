using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.CQRS.Persistence.SQL;
using PinetreeShop.Domain.Baskets.Events;
using PinetreeShop.Domain.Orders.Events;
using PinetreeShop.Domain.Products.Events;
using System.IO;

namespace PinetreeShop.Domain.OrderProcess.Listeners
{
    public class EventListener
    {
        private string _lastEventNumberFile = "lastEvent.txt";
        private EventStreamListener _eventStreamListener;
        private OrderProcessEventHandler _orderProcessEventHandler;
        private IProcessManagerRepository _processManagerRepository;

        public EventListener()
        {
            var eventStore = new SqlEventStore();
            _eventStreamListener = new EventStreamListener(eventStore);

            _processManagerRepository = new ProcessManagerRepository(eventStore, eventStore);
            _orderProcessEventHandler = new OrderProcessEventHandler(_processManagerRepository);

            _eventStreamListener.RegisterEventHandler<BasketCheckedOut>(OnBasketCheckedOut);
            _eventStreamListener.RegisterEventHandler<ProductReserved>(OnProductReserved);
            _eventStreamListener.RegisterEventHandler<ProductReservationFailed>(OnProductReservationFailed);
            _eventStreamListener.RegisterEventHandler<OrderCreated>(OnOrderCreated);
            _eventStreamListener.RegisterEventHandler<OrderCancelled>(OnOrderCancelled);
            _eventStreamListener.RegisterEventHandler<CreateOrderFailed>(OnCreateOrderFailed);
            _eventStreamListener.RegisterEventHandler<OrderDelivered>(OnOrderDelivered);
            _eventStreamListener.RegisterEventHandler<OrderShipped>(OnOrderShipped);
        }

        public void ProcessEvents()
        {
            int lastEventNumber = GetLastEventNumber();

            _eventStreamListener.ReadAndHandleLatestEvents(lastEventNumber);
        }

        private int GetLastEventNumber()
        {
            if (File.Exists(_lastEventNumberFile))
            {
                var number = File.ReadAllText(_lastEventNumberFile);
                return int.Parse(number);
            }
            else
            {
                return 0;
            }
        }

        private void SetLastEventNumber(IEvent evt)
        {
            var number = evt.Metadata.EventNumber;
            File.WriteAllText(_lastEventNumberFile, number.ToString());
        }

        private void OnBasketCheckedOut(BasketCheckedOut evt)
        {
            _orderProcessEventHandler.HandleEvent<BasketCheckedOut, OrderProcessManager>(evt);
            SetLastEventNumber(evt);
        }

        private void OnProductReserved(ProductReserved evt)
        {
            _orderProcessEventHandler.HandleEvent<ProductReserved, OrderProcessManager>(evt);
            SetLastEventNumber(evt);
        }

        private void OnProductReservationFailed(ProductReservationFailed evt)
        {
            _orderProcessEventHandler.HandleEvent<ProductReservationFailed, OrderProcessManager>(evt);
            SetLastEventNumber(evt);
        }

        private void OnOrderCreated(OrderCreated evt)
        {
            _orderProcessEventHandler.HandleEvent<OrderCreated, OrderProcessManager>(evt);
            SetLastEventNumber(evt);
        }

        private void OnOrderCancelled(OrderCancelled evt)
        {
            _orderProcessEventHandler.HandleEvent<OrderCancelled, OrderProcessManager>(evt);
            SetLastEventNumber(evt);
        }

        private void OnCreateOrderFailed(CreateOrderFailed evt)
        {
            _orderProcessEventHandler.HandleEvent<CreateOrderFailed, OrderProcessManager>(evt);
            SetLastEventNumber(evt);
        }

        private void OnOrderDelivered(OrderDelivered evt)
        {
            _orderProcessEventHandler.HandleEvent<OrderDelivered, OrderProcessManager>(evt);
            SetLastEventNumber(evt);
        }

        private void OnOrderShipped(OrderShipped evt)
        {
            _orderProcessEventHandler.HandleEvent<OrderShipped, OrderProcessManager>(evt);
            SetLastEventNumber(evt);
        }
    }
}
