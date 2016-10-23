using PinetreeShop.CQRS.CommandAPI.Models;
using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Persistence.SQL;
using PinetreeShop.Domain.Products;
using PinetreeShop.Domain.Products.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PinetreeShop.CQRS.CommandAPI.Controllers
{
    [RoutePrefix("products")]
    public class ProductsController : ApiController
    {
        IEventStore eventStore = new SqlEventStore();
        
    }
}
