using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.Domain.Baskets;
using PinetreeShop.Domain.Baskets.Events;
using PinetreeShop.Domain.OrderProcess.Commands;
using PinetreeShop.Domain.Orders;
using PinetreeShop.Domain.Orders.Commands;
using PinetreeShop.Domain.Orders.Events;
using PinetreeShop.Domain.Products;
using PinetreeShop.Domain.Products.Commands;
using PinetreeShop.Domain.Products.Events;
using PinetreeShop.Domain.Shared.Types;
using System;
using System.Collections.Generic;
using Xunit;

namespace PinetreeShop.Domain.OrderProcess.Tests
{
    public class OrderProcessTests : OrderProcessTestBase
    {
        Guid orderId = Guid.NewGuid();
        Guid basketId = Guid.NewGuid();
        Guid productOneId = Guid.NewGuid();
        Guid productTwoId = Guid.NewGuid();
        Guid causationAndCorrelationId = Guid.NewGuid();
        Address shippingAddress = new Address { Country = "US", StateOrProvince = "CA", StreetAndNumber = "A2", ZipAndCity = "LA" };

        [Fact]
        public void When_BasketCheckedOut_CreateOrder()
        {
            AggregateRepository.CreateGuid = () => orderId;

            SetupPreviousEvents();
            var nonProcessedEvent = new BasketCheckedOut(basketId, OrderLines, shippingAddress);
            orderId = AddPreviousEvent<BasketAggregate>(nonProcessedEvent);
            SetInitalMetadata();

            Given(_initialEvents.ToArray());
            WhenProcessed(nonProcessedEvent);

            var expectedCommands = new List<ICommand> { new CreateOrder(orderId, basketId, shippingAddress) };
            var expectedEvents = new List<IEvent> { new EventProcessed(orderId, nonProcessedEvent) };

            SetMetadata(nonProcessedEvent, expectedCommands, expectedEvents);

            Then(expectedCommands.ToArray());
            Then(expectedEvents.ToArray());
        }

        [Fact]
        public void When_OrderCreated_ReserveProducts()
        {
            AggregateRepository.CreateGuid = () => orderId;
            SetupPreviousEvents();

            orderId = AddProcessedEvent<BasketAggregate>(new BasketCheckedOut(basketId, OrderLines, shippingAddress));
            var nonProcessedEvent = new OrderCreated(orderId, basketId, shippingAddress);
            AddPreviousEvent<OrderAggregate>(nonProcessedEvent);
            SetInitalMetadata();

            Given(_initialEvents.ToArray());

            WhenProcessed(nonProcessedEvent);

            var expectedCommands = new List<ICommand> { new ReserveProduct(productOneId, 10), new ReserveProduct(productTwoId, 20), };
            var expectedEvents = new List<IEvent> { new EventProcessed(orderId, nonProcessedEvent) };
            SetMetadata(nonProcessedEvent, expectedCommands, expectedEvents);

            Then(expectedCommands.ToArray());
            Then(expectedEvents.ToArray());
        }

        [Fact]
        public void When_ProductReserved_AddOrderLine()
        {
            SetupPreviousEvents();
            orderId = AddProcessedEvent<BasketAggregate>(new BasketCheckedOut(basketId, OrderLines, shippingAddress));
            AddProcessedEvent<OrderAggregate>(new OrderCreated(orderId, basketId, shippingAddress), orderId);

            var nonProcessedEvent = new ProductReserved(productTwoId, 20);
            AddPreviousEvent<ProductAggregate>(nonProcessedEvent);
            SetInitalMetadata();

            Given(_initialEvents.ToArray());

            WhenProcessed(nonProcessedEvent);

            var expectedCommands = new List<ICommand> { new AddOrderLine(orderId, OrderLines[1]) };
            var expectedEvents = new List<IEvent> { new EventProcessed(orderId, nonProcessedEvent) };
            SetMetadata(nonProcessedEvent, expectedCommands, expectedEvents);

            Then(expectedCommands.ToArray());
            Then(expectedEvents.ToArray());
        }


        [Fact]
        public void When_ProductReservedOrderCanceled_RevertReservation()
        {
            SetupPreviousEvents();
            orderId = AddProcessedEvent<BasketAggregate>(new BasketCheckedOut(basketId, OrderLines, shippingAddress));
            AddProcessedEvent<OrderAggregate>(new OrderCreated(orderId, basketId, shippingAddress), orderId);
            AddProcessedEvent<OrderAggregate>(new OrderCancelled(orderId), orderId);

            var nonProcessedEvent = new ProductReserved(productTwoId, 20);
            AddPreviousEvent<ProductAggregate>(nonProcessedEvent);
            SetInitalMetadata();

            Given(_initialEvents.ToArray());

            WhenProcessed(nonProcessedEvent);

            var expectedCommands = new List<ICommand> { new CancelProductReservation(productTwoId, 20) };
            var expectedEvents = new List<IEvent> { new EventProcessed(orderId, nonProcessedEvent) };
            SetMetadata(nonProcessedEvent, expectedCommands, expectedEvents);

            Then(expectedCommands.ToArray());
            Then(expectedEvents.ToArray());
        }

        [Fact]
        public void When_AllProductsReserved_PrepareOrderForShipping()
        {
            var notificationId = Guid.NewGuid();
            AggregateRepository.CreateGuid = () => notificationId;
            SetupPreviousEvents();

            orderId = AddProcessedEvent<BasketAggregate>(new BasketCheckedOut(basketId, OrderLines, shippingAddress));
            AddProcessedEvent<OrderAggregate>(new OrderCreated(orderId, basketId, shippingAddress), orderId);
            AddProcessedEvent<ProductAggregate>(new ProductReserved(productOneId, 10), orderId);
            AddProcessedEvent<OrderAggregate>(new OrderLineAdded(orderId, OrderLines[0]), orderId);

            var nonProcessedEvent = new ProductReserved(productTwoId, 20);
            AddPreviousEvent<ProductAggregate>(nonProcessedEvent);
            SetInitalMetadata();

            Given(_initialEvents.ToArray());

            WhenProcessed(nonProcessedEvent);

            var expectedCommands = new List<ICommand> { new AddOrderLine(orderId, OrderLines[1]), new PrepareOrderForShipping(orderId) };
            var expectedEvents = new List<IEvent> { new EventProcessed(orderId, nonProcessedEvent) };
            SetMetadata(nonProcessedEvent, expectedCommands, expectedEvents);

            Then(expectedCommands.ToArray());
            Then(expectedEvents.ToArray());
        }

        [Fact]
        public void When_SomeProductReservationFailed_NotifyAdmin()
        {
            var notificationId = Guid.NewGuid();
            AggregateRepository.CreateGuid = () => notificationId;
            SetupPreviousEvents();

            orderId = AddProcessedEvent<BasketAggregate>(new BasketCheckedOut(basketId, OrderLines, shippingAddress));
            AddProcessedEvent<OrderAggregate>(new OrderCreated(orderId, basketId, shippingAddress), orderId);
            AddProcessedEvent<ProductAggregate>(new ProductReserved(productOneId, 10), orderId);

            var evt = new ProductReservationFailed(productOneId, 20, ProductReservationFailed.NotAvailable);
            AddPreviousEvent<ProductAggregate>(evt);
            SetInitalMetadata();

            Given(_initialEvents.ToArray());

            WhenProcessed(evt);

            var expectedCommands = new List<ICommand> { new NotifyAdmin(notificationId) };
            var expectedEvents = new List<IEvent> { new EventProcessed(orderId, evt) };
            SetMetadata(evt, expectedCommands, expectedEvents);

            Then(expectedCommands.ToArray());
            Then(expectedEvents.ToArray());
        }

        [Fact]
        public void When_OrderCancelled_CancelProductReservation_And_NotifyCustomer()
        {
            var notificationId = Guid.NewGuid();
            AggregateRepository.CreateGuid = () => notificationId;
            SetupPreviousEvents();

            orderId = AddProcessedEvent<BasketAggregate>(new BasketCheckedOut(basketId, OrderLines, shippingAddress));
            AddProcessedEvent<OrderAggregate>(new OrderCreated(orderId, basketId, shippingAddress), orderId);
            AddProcessedEvent<ProductAggregate>(new ProductReserved(productOneId, 10), orderId);
            AddProcessedEvent<ProductAggregate>(new ProductReserved(productTwoId, 20), orderId);
            AddProcessedEvent<OrderAggregate>(new OrderLineAdded(orderId, OrderLines[0]), orderId);
            AddProcessedEvent<OrderAggregate>(new OrderLineAdded(orderId, OrderLines[1]), orderId);

            var evt = new OrderCancelled(orderId);
            AddPreviousEvent<OrderAggregate>(evt);
            SetInitalMetadata();

            Given(_initialEvents.ToArray());

            WhenProcessed(evt);

            var expectedCommands = new List<ICommand>
            {
                new CancelProductReservation(productOneId, 10),
                new CancelProductReservation(productTwoId, 20),
                new NotifyCustomer(notificationId)
            };
            var expectedEvents = new List<IEvent> { new EventProcessed(orderId, evt) };
            SetMetadata(evt, expectedCommands, expectedEvents);

            Then(expectedCommands.ToArray());
            Then(expectedEvents.ToArray());
        }

        [Fact]
        public void When_OrderCancelled_And_SomeReservationsFailed_DecreaseProductQuantity_And_NotifyCustomer()
        {
            var notificationId = Guid.NewGuid();
            AggregateRepository.CreateGuid = () => notificationId;
            SetupPreviousEvents();

            orderId = AddProcessedEvent<BasketAggregate>(new BasketCheckedOut(basketId, OrderLines, shippingAddress));
            AddProcessedEvent<OrderAggregate>(new OrderCreated(orderId, basketId, shippingAddress), orderId);
            AddProcessedEvent<ProductAggregate>(new ProductReserved(productOneId, 10), orderId);
            AddProcessedEvent<ProductAggregate>(new ProductReservationFailed(productTwoId, 20, ProductReservationFailed.NotAvailable), orderId);
            AddProcessedEvent<OrderAggregate>(new OrderLineAdded(orderId, OrderLines[0]), orderId);
            AddProcessedEvent<OrderAggregate>(new OrderLineAdded(orderId, OrderLines[1]), orderId);

            var evt = new OrderCancelled(orderId);
            AddPreviousEvent<OrderAggregate>(evt);
            SetInitalMetadata();

            Given(_initialEvents.ToArray());

            WhenProcessed(evt);

            var expectedCommands = new List<ICommand>
            {
                new CancelProductReservation(productOneId, 10),
                new NotifyCustomer(notificationId)
            };
            var expectedEvents = new List<IEvent> { new EventProcessed(orderId, evt) };
            SetMetadata(evt, expectedCommands, expectedEvents);

            Then(expectedCommands.ToArray());
            Then(expectedEvents.ToArray());
        }

        [Fact]
        public void When_OrderShipped_DecreaseProductQuantity_And_NotifyCustomer()
        {
            var notificationId = Guid.NewGuid();
            AggregateRepository.CreateGuid = () => notificationId;
            SetupPreviousEvents();

            orderId = AddProcessedEvent<BasketAggregate>(new BasketCheckedOut(basketId, OrderLines, shippingAddress));
            AddProcessedEvent<OrderAggregate>(new OrderCreated(orderId, basketId, shippingAddress), orderId);
            AddProcessedEvent<ProductAggregate>(new ProductReserved(productOneId, 10), orderId);
            AddProcessedEvent<OrderAggregate>(new OrderLineAdded(orderId, OrderLines[0]), orderId);
            AddProcessedEvent<ProductAggregate>(new ProductReserved(productTwoId, 20), orderId);
            AddProcessedEvent<OrderAggregate>(new OrderLineAdded(orderId, OrderLines[1]), orderId);
            AddProcessedEvent<OrderAggregate>(new OrderReadyForShipping(orderId), orderId);

            var evt = new OrderShipped(orderId);
            AddPreviousEvent<OrderAggregate>(evt);
            SetInitalMetadata();

            Given(_initialEvents.ToArray());

            WhenProcessed(evt);

            var expectedCommands = new List<ICommand>
            {
                new PurchaseReservedProduct(productOneId, 10),
                new PurchaseReservedProduct(productTwoId, 20),
                new NotifyCustomer(notificationId)
            };
            var expectedEvents = new List<IEvent> { new EventProcessed(orderId, evt) };
            SetMetadata(evt, expectedCommands, expectedEvents);

            Then(expectedCommands.ToArray());
            Then(expectedEvents.ToArray());
        }

        [Fact]
        public void When_OrderShipped_And_SomeReservationsFailed_DecreaseProductQuantity_And_NotifyCustomer()
        {
            var notificationId = Guid.NewGuid();
            AggregateRepository.CreateGuid = () => notificationId;
            SetupPreviousEvents();

            orderId = AddProcessedEvent<BasketAggregate>(new BasketCheckedOut(basketId, OrderLines, shippingAddress));
            AddProcessedEvent<OrderAggregate>(new OrderCreated(orderId, basketId, shippingAddress), orderId);
            AddProcessedEvent<ProductAggregate>(new ProductReserved(productOneId, 10), orderId);
            AddProcessedEvent<OrderAggregate>(new OrderLineAdded(orderId, OrderLines[0]), orderId);
            AddProcessedEvent<ProductAggregate>(new ProductReservationFailed(productTwoId, 20, ProductReservationFailed.NotAvailable), orderId);
            AddProcessedEvent<OrderAggregate>(new OrderLineAdded(orderId, OrderLines[1]), orderId);
            AddProcessedEvent<OrderAggregate>(new OrderReadyForShipping(orderId), orderId);

            var evt = new OrderShipped(orderId);
            AddPreviousEvent<OrderAggregate>(evt);
            SetInitalMetadata();

            Given(_initialEvents.ToArray());

            WhenProcessed(evt);

            var expectedCommands = new List<ICommand>
            {
                new PurchaseReservedProduct(productOneId, 10),
                new NotifyCustomer(notificationId)
            };
            var expectedEvents = new List<IEvent> { new EventProcessed(orderId, evt) };
            SetMetadata(evt, expectedCommands, expectedEvents);

            Then(expectedCommands.ToArray());
            Then(expectedEvents.ToArray());
        }

        [Fact]
        public void When_OrderDelivered_NotifyAdmin()
        {
            var notificationId = Guid.NewGuid();
            AggregateRepository.CreateGuid = () => notificationId;
            SetupPreviousEvents();

            orderId = AddProcessedEvent<BasketAggregate>(new BasketCheckedOut(basketId, OrderLines, shippingAddress));
            AddProcessedEvent<OrderAggregate>(new OrderCreated(orderId, basketId, shippingAddress), orderId);
            AddProcessedEvent<ProductAggregate>(new ProductReserved(productOneId, 10), orderId);
            AddProcessedEvent<OrderAggregate>(new OrderLineAdded(orderId, OrderLines[0]), orderId);
            AddProcessedEvent<ProductAggregate>(new ProductReserved(productTwoId, 20), orderId);
            AddProcessedEvent<OrderAggregate>(new OrderLineAdded(orderId, OrderLines[1]), orderId);
            AddProcessedEvent<OrderAggregate>(new OrderReadyForShipping(orderId), orderId);
            AddProcessedEvent<OrderAggregate>(new OrderShipped(orderId), orderId);

            var evt = new OrderDelivered(orderId);
            AddPreviousEvent<OrderAggregate>(evt);
            SetInitalMetadata();

            Given(_initialEvents.ToArray());

            WhenProcessed(evt);

            var expectedCommands = new List<ICommand> { new NotifyAdmin(notificationId) };
            var expectedEvents = new List<IEvent> { new EventProcessed(orderId, evt) };
            SetMetadata(evt, expectedCommands, expectedEvents);

            Then(expectedCommands.ToArray());
            Then(expectedEvents.ToArray());
        }

        private void SetupPreviousEvents()
        {
            AddPreviousEvent<BasketAggregate>(new BasketCreated(basketId));
            AddPreviousEvent<BasketAggregate>(new BasketItemAdded(basketId, productOneId, "Test Product 1", 2, 10));
            AddPreviousEvent<BasketAggregate>(new BasketItemAdded(basketId, productTwoId, "Test Product 2", 2, 20));
        }

        private List<OrderLine> OrderLines
        {
            get
            {
                return new List<OrderLine>
                {
                    new OrderLine { ProductId = productOneId, Price = 2, ProductName = "Test Product 1", Quantity = 10 },
                    new OrderLine { ProductId = productTwoId, Price = 2, ProductName = "Test Product 2", Quantity = 20 },
                };
            }
        }

        private void SetMetadata(IEvent evt, List<ICommand> expectedCommands, List<IEvent> expectedEvents)
        {
            expectedCommands.ForEach(x =>
            {
                x.Metadata.CausationId = evt.Metadata.EventId;
                x.Metadata.CorrelationId = evt.Metadata.CorrelationId;
                x.Metadata.ProcessId = orderId;
            });
            expectedEvents.ForEach(x =>
            {
                x.Metadata.CausationId = evt.Metadata.EventId;
                x.Metadata.CorrelationId = evt.Metadata.CorrelationId;
                x.Metadata.ProcessId = orderId;
            });
        }

        private void SetInitalMetadata()
        {
            _initialEvents.ForEach(e =>
            {
                e.Item2.Metadata.CausationId = causationAndCorrelationId;
                e.Item2.Metadata.CorrelationId = causationAndCorrelationId;
                e.Item2.Metadata.ProcessId = orderId;
            });
        }
    }
}
