using PinetreeCQRS.Infrastructure;
using PinetreeCQRS.Infrastructure.Repositories;
using PinetreeCQRS.Persistence.SQL;
using PinetreeShop.Domain.Baskets.Commands;
using PinetreeShop.Domain.Baskets.WebAPI.Models;
using System;
using System.Web.Http;

namespace PinetreeShop.Domain.Baskets.WebAPI.Controllers
{
    [RoutePrefix("")]
    public class BasketController : ApiController
    {
        IEventStore _eventStore = new SqlEventStore();
        string _queueName = typeof(BasketAggregate).Name;
        private BasketCommandDispatcher _commandDispatcher;
        private AggregateRepository _aggregateRepository;

        public BasketController()
        {
            _aggregateRepository = new AggregateRepository(_eventStore);
            _commandDispatcher = new BasketCommandDispatcher(_aggregateRepository);
        }

        [Route("{basketId}"), HttpGet]
        public IHttpActionResult GetBasket(Guid basketId)
        {
            var basket = _aggregateRepository.GetAggregateById<BasketAggregate>(basketId);

            if(basket == null)
            {
                return CreateBasket();
            }

            return Ok(BasketModel.FromAggregate(basket));
        }

        [Route("create"), HttpPost]
        public IHttpActionResult CreateBasket()
        {
            var basketId = Guid.NewGuid();
            var cmd = new CreateBasket(basketId);
            var basket = _commandDispatcher.ExecuteCommand<BasketAggregate>(cmd);

            return Ok(BasketModel.FromAggregate(basket));
        }

        [Route("addItem"), HttpPost]
        public IHttpActionResult AddItem([FromBody] AddItemModel model)
        {
            var basketId = model.BasketId;
            if(model.BasketId == Guid.Empty)
            {
                basketId = Guid.NewGuid();
                _commandDispatcher.ExecuteCommand<BasketAggregate>(new CreateBasket(basketId));
            }

            // TODO: get model price and name from api by id
            var cmd = new AddItemToBasket(
                basketId,
                model.ProductId,
                model.Name,
                model.Price,
                model.Quantity);

            var basket = _commandDispatcher.ExecuteCommand<BasketAggregate>(cmd);

            return Ok(BasketModel.FromAggregate(basket));
        }

        [Route("removeItem"), HttpPost]
        public IHttpActionResult RemoveItem([FromBody] RemoveItemModel model)
        {
            var cmd = new RemoveItemFromBasket(
                model.BasketId,
                model.ProductId,
                model.Quantity);

            var basket = _commandDispatcher.ExecuteCommand<BasketAggregate>(cmd);

            return Ok(BasketModel.FromAggregate(basket));
        }

        [Route("checkout"), HttpPost]
        public IHttpActionResult CheckOut([FromBody] CheckOutModel model)
        {
            var cmd = new CheckOutBasket(model.BasketId, model.ShippingAddress);

            var basket = _commandDispatcher.ExecuteCommand<BasketAggregate>(cmd);

            return Ok(BasketModel.FromAggregate(basket));
        }

        [Route("cancel"), HttpPost]
        public IHttpActionResult Cancel([FromBody] CancelModel model)
        {
            var cmd = new CancelBasket(model.BasketId);
            var basket = _commandDispatcher.ExecuteCommand<BasketAggregate>(cmd);

            return Ok();
        }
    }
}
