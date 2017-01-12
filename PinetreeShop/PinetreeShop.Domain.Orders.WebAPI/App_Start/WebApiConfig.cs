using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace PinetreeShop.Domain.Orders.WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            var cors = new EnableCorsAttribute(Configuration.Instance.ClientUrl, "*", "*");
            config.EnableCors(cors);
        }
    }
}
