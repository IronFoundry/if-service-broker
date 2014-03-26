using System.Web.Http;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace IronFoundry.ServiceBroker
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var serializerSettings = config.Formatters.JsonFormatter.SerializerSettings;
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            serializerSettings.Converters.Add(new StringEnumConverter());

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.EnableSystemDiagnosticsTracing();
            config.Services.Replace(typeof(System.Web.Http.Tracing.ITraceWriter), new WebApiContrib.Tracing.Nlog.NlogTraceWriter());
        }
    }
}