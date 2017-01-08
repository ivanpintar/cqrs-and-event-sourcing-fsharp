using Chessie.ErrorHandling;
using PinetreeShop.Domain.Baskets.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using static PinetreeCQRS.Infrastructure.Types;
using static PinetreeShop.Domain.Baskets.BasketAggregate;
using static PinetreeCQRS.Infrastructure.Commands;
using static PinetreeCQRS.Persistence.SqlServer;

namespace PinetreeShop.Domain.Baskets.WebAPI.Controllers
{
    [RoutePrefix("")]
    public class BasketController : ApiController
    {
        public BasketController()
        {
        }

        [Route("{basketId}"), HttpGet]
        public IHttpActionResult GetBasket(Guid basketId)
        {
            var basket = GetBasketAggregate(basketId);

            if(basket == null)
            {
                return CreateBasket();
            }

            return Ok(basket);
        }

        [Route("create"), HttpPost]
        public IHttpActionResult CreateBasket()
        {
            var basketId = Guid.NewGuid();
            var cmd = Command.Create;
            var envelope = createCommand(AggregateId.NewAggregateId(basketId), AggregateVersion.NewExpected(0), null, null, null, cmd);

            var basket = CommitCommand(envelope);

            return Ok(basket);
        }

        [Route("addItem"), HttpPost]
        public IHttpActionResult AddItem([FromBody] AddItemModel model)
        {
            var basketId = model.BasketId; 
            if (model.BasketId == Guid.Empty)
            {
                basketId = Guid.NewGuid();
                var cmd = Command.Create;
                var envelope = createCommand(AggregateId.NewAggregateId(basketId), AggregateVersion.NewExpected(0), null, null, null, cmd);

                CommitCommand(envelope);
            }

            // TODO: get model price and name from api by id
            var productId = model.ProductId;
            var item = new BasketItem(ProductId.NewProductId(productId), model.Name, model.Price, model.Quantity);
            var addCmd = Command.NewAddItem(item);
            var addEnvelope = createCommand(AggregateId.NewAggregateId(basketId), AggregateVersion.Irrelevant, null, null, null, addCmd);
            
            var basket = CommitCommand(addEnvelope);

            return Ok(basket);
        }

        [Route("removeItem"), HttpPost]
        public IHttpActionResult RemoveItem([FromBody] RemoveItemModel model)
        {
            var basketId = model.BasketId;
            var productId = model.ProductId;
            var cmd = Command.NewRemoveItem(ProductId.NewProductId(productId), model.Quantity);
            var envelope = createCommand(AggregateId.NewAggregateId(basketId), AggregateVersion.Irrelevant, null, null, null, cmd);

            CommitCommand(envelope);

            return Ok();
        }

        [Route("checkout"), HttpPost]
        public IHttpActionResult CheckOut([FromBody] CheckOutModel model)
        {
            var basketId = model.BasketId;
            var address = model.ShippingAddress;
            var cmd = Command.NewCheckOut(ShippingAddress.NewShippingAddress(address));
            var envelope = createCommand(AggregateId.NewAggregateId(basketId), AggregateVersion.Irrelevant, null, null, null, cmd);

            var basket = CommitCommand(envelope);

            return Ok(basket);
        }

        [Route("cancel"), HttpPost]
        public IHttpActionResult Cancel([FromBody] CancelModel model)
        {
            var basketId = model.BasketId;
            var cmd = Command.Cancel;
            var envelope = createCommand(AggregateId.NewAggregateId(basketId), AggregateVersion.Irrelevant, null, null, null, cmd);

            CommitCommand(envelope);

            return Ok();
        }

        private BasketModel CommitCommand(CommandEnvelope<Command> cmd)
        {
            var list = new List<CommandEnvelope<Command>> { cmd };
            var res = CommandHandler.handler.Invoke(cmd);

            if (res.IsOk)
            {
                return GetBasketAggregate(cmd.AggregateId.Item);
            }

            var f = (res as Result<IEnumerable<EventEnvelope<Event>>, IError>.Bad).Item;

            var reasons = f.Select(x => x.ToString()).ToArray();
            var reason = string.Join("; ", reasons);
            throw new Exception(reason);
        }

        private BasketModel GetBasketAggregate(Guid basketId)
        {
            var res = Reader.getBasket(AggregateId.NewAggregateId(basketId));
            if (res.IsOk)
            {
                var b = (res as Result<State, IError>.Ok).Item1;
                return BasketModel.FromAggregate(basketId, b);
            }

            return null;
        }
    }
}
