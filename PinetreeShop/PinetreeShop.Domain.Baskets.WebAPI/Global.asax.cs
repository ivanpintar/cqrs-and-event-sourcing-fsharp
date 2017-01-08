using Newtonsoft.Json.Serialization;
using System.Web;
using System.Web.Http;

namespace PinetreeShop.Domain.Baskets.WebAPI
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configuration
                .Formatters
                .JsonFormatter
                .SerializerSettings
                .ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}
