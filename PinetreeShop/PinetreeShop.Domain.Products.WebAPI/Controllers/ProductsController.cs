using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.CQRS.Persistence.SQL;
using PinetreeShop.Domain.Products.Commands;
using PinetreeShop.Domain.Products.ReadModel;
using PinetreeShop.Domain.Products.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace PinetreeShop.Domain.Products.WebAPI.Controllers
{
    [RoutePrefix("")]
    public class ProductsController : ApiController
    {
        IEventStore _eventStore = new SqlEventStore();
        string _queueName = typeof(ProductAggregate).Name;
        private ProductCommandDispatcher _commandDispatcher;

        public ProductsController()
        {
            _commandDispatcher = new ProductCommandDispatcher(new AggregateRepository(_eventStore));
        }
        
        [Route("list"), HttpGet]
        public List<ProductModel> GetProducts() 
        {
            using(var ctx = new ProductContext())
            {
                return ctx.Products.Select(ProductModel.FromEntity).ToList();
            }
        }
        
        [Route("create"), HttpPost]
        public IHttpActionResult CreateProduct([FromBody] CreateProductModel model)
        {
            var productId = Guid.NewGuid();

            var cmd = new CreateProduct(productId, model.Name, model.Price);
            var product = _commandDispatcher.ExecuteCommand<ProductAggregate>(cmd);

            if(model.Quantity > 0)
            {
                return ChangeQuantity(new SetQuantityModel
                {
                    Id = productId,
                    Quantity = model.Quantity
                });
            }

            return Ok(ProductModel.FromAggregate(product));
        }
        
        [Route("quantity"), HttpPost]
        public IHttpActionResult ChangeQuantity([FromBody] SetQuantityModel model)
        {
            var cmd = new SetProductQuantity(model.Id, model.Quantity);
            var product = _commandDispatcher.ExecuteCommand<ProductAggregate>(cmd);

            return Ok(ProductModel.FromAggregate(product));
        }
    }
}
