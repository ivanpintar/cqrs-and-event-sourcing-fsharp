using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace PinetreeShop.WebAPI.Controllers
{
    [RoutePrefix("basket")]
    public class BasketsController : ApiController
    {
        [HttpPost]
        [Route("create")]
        public Guid Create()
        {
            return Guid.NewGuid();
        }

        [HttpPost]
        [Route("addProduct/{productId}/{quantity}")]
        public void AddProduct(Guid productId, uint quantity)
        {

        }

        [HttpPost]
        [Route("removeProduct/{productId}/{quantity}")]
        public void RemoveProduct(Guid productId, uint quantity)
        {

        }

        [HttpPost]
        [Route("cancel/{basketId}")]
        public void Cancel(Guid basketId)
        {

        }

        [HttpPost]
        [Route("checkOut/{basketId}")]
        public void CheckOut(Guid basketId)
        {

        }
        
    }
}