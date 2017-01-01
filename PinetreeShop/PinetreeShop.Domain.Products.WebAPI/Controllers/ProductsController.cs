using PinetreeShop.Domain.Products.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using static PinetreeShop.Domain.Products.ProductAggregate;
using static PinetreeShop.Domain.Products.CommandHandler;
using static PinetreeCQRS.Infrastructure.Commands;
using Microsoft.FSharp.Core;
using static PinetreeCQRS.Infrastructure.Types;
using static PinetreeShop.Domain.Products.ReadModel;
using Chessie.ErrorHandling;

namespace PinetreeShop.Domain.Products.WebAPI.Controllers
{
    [RoutePrefix("")]
    public class ProductsController : ApiController
    {
        [Route("list"), HttpGet]
        public IHttpActionResult GetProducts()
        {
            var products = Reader.getProducts().Select(ProductModel.FromDTO).ToList();
            return Ok(products);
        }

        [Route("create"), HttpPost]
        public IHttpActionResult CreateProduct([FromBody] CreateProductModel model)
        {
            var id = AggregateId.NewAggregateId(Guid.NewGuid());
            var versionNumber = AggregateVersion.NewExpected(0);
            var cmd = Command.NewCreate(model.Name, model.Price);
            var envelope = createCommand(id, versionNumber, null, null, null, cmd);

            var res = HandleCommand(envelope);

            if (model.Quantity > 0)
            {
                cmd = Command.NewAddToStock(model.Quantity);
                versionNumber = AggregateVersion.NewExpected(res.Last().EventNumber);
                envelope = createCommand(id, versionNumber, null, null, null, cmd);
                HandleCommand(envelope);
            }

            return Ok(new ProductModel
            {
                Id = id.Item,
                Name = model.Name,
                Price = model.Price,
                Quantity = model.Quantity,
                Reserved = 0
            });
        }

        [Route("quantity"), HttpPost]
        public IHttpActionResult ChangeQuantity([FromBody] ChangeQuantityModel model)
        {
            var id = AggregateId.NewAggregateId(model.Id);
            var versionNumber = AggregateVersion.Irrelevant;
            var cmd = model.Difference >= 0
                ? Command.NewAddToStock(model.Difference)
                : Command.NewRemoveFromStock(model.Difference);

            var envelope = createCommand(id, versionNumber, null, null, null, cmd);
            HandleCommand(envelope);

            return Ok();
        }

        private IEnumerable<EventEnvelope<Event>> HandleCommand(CommandEnvelope<Command> cmd)
        {
            var res = handleCommand.Invoke(cmd);

            if (res.IsOk)
            {
                var r = (res as Result<IEnumerable<EventEnvelope<Event>>, IError>.Ok).Item1;
                return r;
            }

            var f = (res as Result<IEnumerable<EventEnvelope<Event>>, IError>.Bad).Item;

            var reasons = f.Select(x => x.ToString()).ToArray();
            var reason = string.Join("; ", reasons);
            throw new Exception(reason);
        }
    }

}
