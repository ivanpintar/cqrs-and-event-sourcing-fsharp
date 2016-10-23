using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace PinetreeShop.Domain.Products.WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();
        }
    }
}
