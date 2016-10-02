using PinetreeShop.WebAPI.Models;
using System;
using System.Web.Http;

namespace PinetreeShop.WebAPI.Controllers
{
    [RoutePrefix("products")]
    public class ProductsController : ApiController
    {
        [HttpPost]
        [Route("create")]
        public object CreateProduct()
        {
            return "";
        }

        [HttpPost]
        [Route("changeQuantity")]
        public object ChangeProductQuantity([FromBody] ChangeQuantityModel model)
        {
            return "";
        }

        [HttpPost]
        [Route("releaseReservation")]
        public object ReleaseProductReservation([FromBody] ReleaseReservationModel model)
        {
            return "";
        }
    }
}