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

            QueueCommand(envelope);

            if (model.Quantity > 0)
            {
                cmd = Command.NewAddToStock(model.Quantity);
                envelope = createCommand(id, AggregateVersion.Irrelevant, null, null, null, cmd);
                QueueCommand(envelope);
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
            QueueCommand(envelope);

            return Ok();
        }

        private void QueueCommand(CommandEnvelope<Command> cmd)
        {
            var list = new List<CommandEnvelope<Command>> { cmd };
            var res = PinetreeCQRS.Persistence.SqlServer.Commands.queueCommand<Command>(list);

            if (res.IsOk)
            {
                return;
            }

            var f = (res as Result<IEnumerable<CommandEnvelope<ICommand>>, IError>.Bad).Item;

            var reasons = f.Select(x => x.ToString()).ToArray();
            var reason = string.Join("; ", reasons);
            throw new Exception(reason);
        }
    }

}
