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
        private AggregateRepository _aggregateRepository;

        public ProductsController()
        {
            _aggregateRepository = new AggregateRepository(_eventStore);
            _commandDispatcher = new ProductCommandDispatcher(_aggregateRepository);
        }
        
        [Route("stuff"), HttpGet]
        public bool GetStuff()
        {
            return true;
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
            _commandDispatcher.ExecuteCommand<ProductAggregate>(cmd);

            if(model.Quantity > 0)
            {
                return ChangeQuantity(new SetQuantityModel
                {
                    Id = productId,
                    Quantity = model.Quantity
                });
            }

            var product = _aggregateRepository.GetAggregateById<ProductAggregate>(productId);
            return Ok(ProductModel.FromAggregate(product));
        }
        
        [Route("quantity"), HttpPost]
        public IHttpActionResult ChangeQuantity([FromBody] SetQuantityModel model)
        {
            var cmd = new SetProductQuantity(model.Id, model.Quantity);
            _commandDispatcher.ExecuteCommand<ProductAggregate>(cmd);

            var product = _aggregateRepository.GetAggregateById<ProductAggregate>(model.Id);
            return Ok(ProductModel.FromAggregate(product));
        }
    }
}
