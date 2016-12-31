using PinetreeShop.Domain.Products.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using PinetreeCQRS.Infrastructure;
using static PinetreeShop.Domain.Products.ProductAggregate.Command;
using static PinetreeShop.Domain.Products.CommandHandler;
using static PinetreeCQRS.Infrastructure.Commands;
using Microsoft.FSharp.Core;
using static PinetreeCQRS.Infrastructure.Types;

namespace PinetreeShop.Domain.Products.WebAPI.Controllers
{
    [RoutePrefix("")]
    public class ProductsController : ApiController
    {
        [Route("list"), HttpGet]
        public List<ProductModel> GetProducts() 
        {
            throw new NotImplementedException();
        }
        
        [Route("create"), HttpPost]
        public IHttpActionResult CreateProduct([FromBody] CreateProductModel model)
        {

            var id = AggregateId.NewAggregateId(Guid.NewGuid());
            var versionNumber = AggregateVersion.NewExpected(0);
            var cmd = NewCreate(model.Name, model.Price);
            var envelope = createCommand(id, versionNumber, null, null, null, cmd);

            var res = HandleCommand(envelope);
           
            if(model.Quantity > 0)
            {
                cmd = NewAddToStock(model.Quantity);
                versionNumber = AggregateVersion.NewExpected(res.Last().eventNumber);
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
            

            return Ok();
        }

        private IEnumerable<EventEnvelope<ProductAggregate.Event>> HandleCommand(CommandEnvelope<ProductAggregate.Command> cmd)
        {
            var res = handleCommand(cmd);

            if (res.IsChoice1Of2)
            {
                var r = (res as FSharpChoice<IEnumerable<EventEnvelope<ProductAggregate.Event>>, CommandFailedEnvelope<ProductAggregate.Command>>.Choice1Of2).Item;
                return r;
            } 

            var f = (res as FSharpChoice<IEnumerable<EventEnvelope<ProductAggregate.Event>>, CommandFailedEnvelope<ProductAggregate.Command>>.Choice2Of2).Item;

            var reasons = f.reasons.Select(x => x.ToString()).ToArray();
            var reason = string.Join("; ", reasons);
            throw new Exception(reason);
        }
    }

}
