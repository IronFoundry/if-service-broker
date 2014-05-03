using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using IronFoundry.ServiceBroker.Models;

namespace IronFoundry.ServiceBroker.Modules
{
    // http://www.asp.net/web-api/overview/security/basic-authentication

    // To enable:
    // <system.webServer>
    //     <modules>
    //       <add name="BasicAuthHttpModule" type="IronFoundry.ServiceBroker.Modules.BasicAuthHttpModule, IronFoundry.ServiceBroker"/>
    //     </modules>

    public class BasicAuthHttpModule : IHttpModule
    {
        private readonly IConfiguration configuration;
        private const string Realm = "";

        public BasicAuthHttpModule()
        {
            this.configuration = new AppConfiguration();
        }

        public BasicAuthHttpModule(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest += OnApplicationAuthenticateRequest;
            context.EndRequest += OnApplicationEndRequest;
        }

        private void OnApplicationAuthenticateRequest(object sender, EventArgs e)
        {
            AuthenticateRequest(new HttpContextWrapper(HttpContext.Current));
        }

        public void AuthenticateRequest(HttpContextBase context)
        {
            var request = context.Request;
            var authHeader = request.Headers["Authorization"];
            if (authHeader == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            var authHeaderVal = AuthenticationHeaderValue.Parse(authHeader);

            if (!authHeaderVal.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase) || authHeaderVal.Parameter == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            if (!AuthenticateUser(context, authHeaderVal.Parameter))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
        }

        private void OnApplicationEndRequest(object sender, EventArgs e)
        {
            EndRequest(new HttpContextWrapper(HttpContext.Current));
        }

        public void EndRequest(HttpContextBase context)
        {
            var response = context.Response;
            if (response.StatusCode == 401)
            {
                // If the request was unauthorized, add the WWW-Authenticate header to the response.
                response.Headers.Add("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", Realm));
            }
        }

        private  void SetPrincipal(HttpContextBase context, IPrincipal principal)
        {
            Thread.CurrentPrincipal = principal;
            if (context != null)
            {
                context.User = principal;
            }
        }

        private bool CheckPassword(string userName, string password)
        {
            var expectedUserName = configuration.GetAppSetting("brokerUserName");
            var expectedPassword = configuration.GetAppSetting("brokerPassword");
            return userName == expectedUserName && password == expectedPassword;
        }

        private bool AuthenticateUser(HttpContextBase context, string credentials)
        {
            bool validated;
            try
            {
                var encoding = Encoding.GetEncoding("iso-8859-1");
                credentials = encoding.GetString(Convert.FromBase64String(credentials));

                int indexOfSeparator = credentials.IndexOf(':');
                string name = credentials.Substring(0, indexOfSeparator);
                string password = credentials.Substring(indexOfSeparator + 1);

                validated = CheckPassword(name, password);
                if (validated)
                {
                    var identity = new GenericIdentity(name);
                    SetPrincipal(context, new GenericPrincipal(identity, null));
                }
            }
            catch (FormatException)
            {
                validated = false;

            }
            return validated;
        }

        public void Dispose()
        {
        }
    }
}