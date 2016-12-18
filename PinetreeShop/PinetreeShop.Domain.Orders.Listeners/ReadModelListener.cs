using PinetreeCQRS.Infrastructure.Events;
using PinetreeCQRS.Persistence.SQL;
using PinetreeShop.Domain.Orders.Events;
using PinetreeShop.Domain.Orders.ReadModel;
using PinetreeShop.Domain.Orders.ReadModel.Entitites;
using System;
using System.Linq;
using PinetreeShop.Domain.Shared.Types;

namespace PinetreeShop.Domain.Orders.Listeners
{
    public class ReadModelListener
    {
        private EventStreamListener _eventStreamListener;
        private OrderContext _ctx;

        public ReadModelListener()
        {
            var eventStore = new SqlEventStore();
            _eventStreamListener = new EventStreamListener(eventStore);

            _eventStreamListener.RegisterEventHandler<OrderCreated>(OnOrderCreated);
            _eventStreamListener.RegisterEventHandler<OrderLineAdded>(OnOrderLineAdded);
            _eventStreamListener.RegisterEventHandler<OrderReadyForShipping>(OnOrderReadyForShipping);
            _eventStreamListener.RegisterEventHandler<OrderCancelled>(OnOrderCancelled);
            _eventStreamListener.RegisterEventHandler<OrderShipped>(OnOrderShipped);
            _eventStreamListener.RegisterEventHandler<OrderDelivered>(OnOrderDelivered);
        }

        private void OnOrderReadyForShipping(OrderReadyForShipping evt)
        {
            var order = GetOrder(evt);
            order.State = "ReadyForShipping";
            order.LastEventNumber = evt.Metadata.EventNumber;
            _ctx.SaveChanges();
        }

        private void OnOrderLineAdded(OrderLineAdded evt)
        {
            var order = GetOrder(evt);
            order.LastEventNumber = evt.Metadata.EventNumber;
            order.Lines.Add(new Line
            {
                Id = Guid.NewGuid(),
                Price = evt.OrderLine.Price,
                ProductId = evt.OrderLine.ProductId,
                ProductName = evt.OrderLine.ProductName,
                Quantity = evt.OrderLine.Quantity
            });

            _ctx.SaveChanges();
        }

        private void OnOrderDelivered(OrderDelivered evt)
        {
            var order = GetOrder(evt);
            order.State = "Deliverred";
            order.LastEventNumber = evt.Metadata.EventNumber;
            _ctx.SaveChanges();
        }

        private void OnOrderShipped(OrderShipped evt)
        {
            var order = GetOrder(evt);
            order.State = "Shipped";
            order.LastEventNumber = evt.Metadata.EventNumber;
            _ctx.SaveChanges();
        }

        private void OnOrderCancelled(OrderCancelled evt)
        {
            var order = GetOrder(evt);
            order.State = "Cancelled";
            order.LastEventNumber = evt.Metadata.EventNumber;
            _ctx.SaveChanges();
        }

        private void OnOrderCreated(OrderCreated evt)
        {
            _ctx.Orders.Add(new Order
            {
                Id = evt.AggregateId,
                State = "Pending",

                StreetAndNumber = evt.ShippingAddress.StreetAndNumber,
                ZipAndCity = evt.ShippingAddress.ZipAndCity,
                StateOrProvince = evt.ShippingAddress.StateOrProvince,
                Country = evt.ShippingAddress.Country,

                LastEventNumber = evt.Metadata.EventNumber
            });
            _ctx.SaveChanges();
        }

        private Line CreateLine(OrderLine arg)
        {
            return new Line
            {
                Id = Guid.NewGuid(),
                ProductId = arg.ProductId,
                Price = arg.Price,
                Quantity = arg.Quantity
            };
        }

        public void ProcessEvents()
        {
            using (_ctx = new OrderContext())
            {
                int lastEventNumber = GetLastEventNumber();
                _eventStreamListener.ReadAndHandleLatestEvents<OrderAggregate>(lastEventNumber);
            }
        }

        private int GetLastEventNumber()
        {
            if (!_ctx.Orders.Any()) return 0;
            return _ctx.Orders.Max(p => p.LastEventNumber);
        }

        private Order GetOrder(IEvent evt)
        {
            return _ctx.Orders.Single(p => p.Id == evt.AggregateId);
        }
    }
}
