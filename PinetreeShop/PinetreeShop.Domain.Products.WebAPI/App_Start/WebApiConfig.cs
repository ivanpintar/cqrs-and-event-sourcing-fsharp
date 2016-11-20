using System.Web.Http;
using System.Web.Http.Cors;

namespace PinetreeShop.Domain.Products.WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();
            
            var cors = new EnableCorsAttribute(Configuration.Instance.ClientUrl, "*", "*");
            config.EnableCors(cors);
        }
    }
}
