using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.CQRS.Persistence.SQL;
using PinetreeShop.Domain.Orders.Commands;
using PinetreeShop.Domain.Orders.ReadModel;
using PinetreeShop.Domain.Orders.WebAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Data.Entity;

namespace PinetreeShop.Domain.Orders.WebAPI.Controllers
{
    [RoutePrefix("")]
    public class OrdersController : ApiController
    {
        IEventStore _eventStore = new SqlEventStore();
        string _queueName = typeof(OrderAggregate).Name;
        private OrderCommandDispatcher _commandDispatcher;

        public OrdersController()
        {
            _commandDispatcher = new OrderCommandDispatcher(new AggregateRepository(_eventStore));
        }

        [Route("list"), HttpGet]
        public List<OrderModel> GetOrders()
        {
            using (var ctx = new OrderContext())
            {
                return ctx.Orders
                    .Include(o => o.Lines)
                    .Select(OrderModel.FromEntity).ToList();
            }
        }

        [Route("cancel"), HttpPost]
        public IHttpActionResult CancelOrder([FromBody] GuidModel model)
        {
            var cmd = new CancelOrder(model.OrderId);
            var order = _commandDispatcher.ExecuteCommand<OrderAggregate>(cmd);

            return Ok(OrderModel.FromAggregate(order));
        }

        [Route("prepareForShipping"), HttpPost]
        public IHttpActionResult PrepareForShipping([FromBody] GuidModel model)
        {
            var cmd = new PrepareOrderForShipping(model.OrderId);
            var order = _commandDispatcher.ExecuteCommand<OrderAggregate>(cmd);

            return Ok(OrderModel.FromAggregate(order));
        }

        [Route("ship"), HttpPost]
        public IHttpActionResult ShipOrder([FromBody] GuidModel model)
        {
            var cmd = new ShipOrder(model.OrderId);
            var order = _commandDispatcher.ExecuteCommand<OrderAggregate>(cmd);

            return Ok(OrderModel.FromAggregate(order));
        }

        [Route("deliver"), HttpPost]
        public IHttpActionResult DeliverOrder([FromBody] GuidModel model)
        {
            var cmd = new DeliverOrder(model.OrderId);
            var order = _commandDispatcher.ExecuteCommand<OrderAggregate>(cmd);

            return Ok(OrderModel.FromAggregate(order));
        }
    }
}
