using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.CQRS.Persistence.SQL;
using PinetreeShop.Domain.Products;
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

        [HttpGet]
        [Route("")]
        public List<ProductModel> GetProducts() 
        {
            using(var ctx = new ProductContext())
            {
                return ctx.Products.Select(ProductModel.FromEntity).ToList();
            }
        }
        
        [HttpPost]
        [Route("create")]
        public bool CreateProduct([FromBody] CreateProductModel model)
        {
            var cmd = new CreateProduct(Guid.NewGuid(), model.Name, model.Price);
            _commandDispatcher.ExecuteCommand<ProductAggregate>(cmd);

            return true;
        }

        [HttpPost]
        [Route("quantity")]
        public bool ChangeQuantity([FromBody] ChangeQuantityModel model)
        {
            var cmd = new ChangeProductQuantity(model.Id, model.Difference);
            _commandDispatcher.ExecuteCommand<ProductAggregate>(cmd);
            return true;
        }
    }
}
