using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NSubstitute;

namespace IronFoundry.ServiceBroker.Tests
{
    public class WebApiTest
    {
        public static readonly string CurrentController = "_CurrentController_";
        public static readonly HttpMethod RequestMethod = HttpMethod.Get;
        public static readonly Uri RequestUri = new Uri("http://localhost:1337/request");

        public WebApiTest()
        {
            var routeData = Substitute.For<IHttpRouteData>();
            var routeValues = new Dictionary<string, object> {{"controller", CurrentController}};
            routeData.Values.Returns(routeValues);
            routeData.Route.Returns(Substitute.For<IHttpRoute>());

            Configuration = new HttpConfiguration();
            Configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            Configuration.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter());
            Routes = Configuration.Routes;

            RequestMessage = new HttpRequestMessage(RequestMethod, RequestUri);
            RequestContext = new TestHttpRequestContext(RequestMessage) {Configuration = Configuration};

            RequestMessage.SetRequestContext(RequestContext);
            RequestMessage.SetConfiguration(Configuration);
            RequestMessage.SetRouteData(routeData);

            ControllerContext = new HttpControllerContext(Configuration, routeData, RequestMessage);
            ControllerDescriptor = Substitute.For<HttpControllerDescriptor>(Configuration, CurrentController, typeof (DummyController));
            ActionDescriptor = Substitute.For<HttpActionDescriptor>(ControllerDescriptor);
            ActionContext = new HttpActionContext(ControllerContext, ActionDescriptor);
        }

        public HttpActionContext ActionContext { get; private set; }
        public HttpActionDescriptor ActionDescriptor { get; private set; }
        public HttpConfiguration Configuration { get; private set; }
        public HttpControllerContext ControllerContext { get; private set; }
        public HttpControllerDescriptor ControllerDescriptor { get; private set; }
        public HttpRequestMessage RequestMessage { get; private set; }
        public HttpRequestContext RequestContext { get; private set; }
        public HttpRouteCollection Routes { get; private set; }
    }

    internal class TestHttpRequestContext : HttpRequestContext
    {
        public TestHttpRequestContext(HttpRequestMessage request)
        {
            Url = new UrlHelper(request);
        }

        public override IPrincipal Principal
        {
            get { return Thread.CurrentPrincipal; }
            set { Thread.CurrentPrincipal = value; }
        }
    }

    internal class DummyController : ApiController
    {
    }
}