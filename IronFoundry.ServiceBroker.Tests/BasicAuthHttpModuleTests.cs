using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI.WebControls;
using IronFoundry.ServiceBroker.Models;
using IronFoundry.ServiceBroker.Modules;
using NSubstitute;
using Xunit;

namespace IronFoundry.ServiceBroker.Tests
{
    public class BasicAuthHttpModuleTests
    {
        [Fact]
        public void DeniesAccessIfNoAuthorizationHeader()
        {
            var basicAuth = new BasicAuthHttpModule(CreateMockConfiguration());
            var context = CreateMockContext();

            basicAuth.AuthenticateRequest(context);

            Assert.Equal((int)HttpStatusCode.Unauthorized, context.Response.StatusCode);
        }

        [Fact]
        public void DeniesAccessIfNotBasicAuth()
        {
            var basicAuth = new BasicAuthHttpModule(CreateMockConfiguration());
            var context = CreateMockContext();
            context.Request.Headers.Add("Authorization", "some other wacky auth scheme");
            basicAuth.AuthenticateRequest(context);

            Assert.Equal((int)HttpStatusCode.Unauthorized, context.Response.StatusCode);
        }

        [Fact]
        public void DeniesAccessIfNoAuthParameter()
        {
            var basicAuth = new BasicAuthHttpModule(CreateMockConfiguration());
            var context = CreateMockContext();
            context.Request.Headers.Add("Authorization", "Basic");
            basicAuth.AuthenticateRequest(context);

            Assert.Equal((int)HttpStatusCode.Unauthorized, context.Response.StatusCode);
        }

        [Fact]
        public void DeniesAccessIfNoUser()
        {
            var basicAuth = new BasicAuthHttpModule(CreateMockConfiguration());
            var context = CreateMockContext();
            context.Request.Headers.Add("Authorization", BuildAuthorizationHeader("", "password"));
            basicAuth.AuthenticateRequest(context);

            Assert.Equal((int)HttpStatusCode.Unauthorized, context.Response.StatusCode);
        }

        [Fact]
        public void DeniesAccessIfNoPassword()
        {
            var basicAuth = new BasicAuthHttpModule(CreateMockConfiguration());
            var context = CreateMockContext();
            context.Request.Headers.Add("Authorization", BuildAuthorizationHeader("user", ""));
            basicAuth.AuthenticateRequest(context);

            Assert.Equal((int)HttpStatusCode.Unauthorized, context.Response.StatusCode);
        }

        [Fact]
        public void DeniesAccessIfNoUserOrPassword()
        {
            var basicAuth = new BasicAuthHttpModule(CreateMockConfiguration());
            var context = CreateMockContext();
            context.Request.Headers.Add("Authorization", BuildAuthorizationHeader("", ""));
            basicAuth.AuthenticateRequest(context);

            Assert.Equal((int)HttpStatusCode.Unauthorized, context.Response.StatusCode);
        }

        [Fact]
        public void GrantsAccessIfCredentialsAreCorrect()
        {
            var basicAuth = new BasicAuthHttpModule(CreateMockConfiguration());
            var context = CreateMockContext();
            context.Request.Headers.Add("Authorization", BuildAuthorizationHeader("user", "password"));
            basicAuth.AuthenticateRequest(context);

            Assert.Equal(0, context.Response.StatusCode);
        }

        [Fact]
        public void GrantsAccessIfPasswordContainsColon()
        {
            var basicAuth = new BasicAuthHttpModule(CreateMockConfiguration("user", "pass:word"));
            var context = CreateMockContext();
            context.Request.Headers.Add("Authorization", BuildAuthorizationHeader("user", "pass:word"));
            basicAuth.AuthenticateRequest(context);

            Assert.Equal(0, context.Response.StatusCode);
        }

        [Fact]
        public void DoesNotSetPrincipalWhenAccessIsDenied()
        {
            var basicAuth = new BasicAuthHttpModule(CreateMockConfiguration());
            var context = CreateMockContext();
            context.Request.Headers.Add("Authorization", BuildAuthorizationHeader("user", "badpassword"));
            Thread.CurrentPrincipal = null;
            basicAuth.AuthenticateRequest(context);

            Assert.Equal("", Thread.CurrentPrincipal.Identity.Name);
            Assert.Equal("", context.User.Identity.Name);
        }

        [Fact]
        public void SetsPrincipalWhenAccessIsGranted()
        {
            var basicAuth = new BasicAuthHttpModule(CreateMockConfiguration());
            var context = CreateMockContext();
            context.Request.Headers.Add("Authorization", BuildAuthorizationHeader("user", "password"));
            Thread.CurrentPrincipal = null;
            basicAuth.AuthenticateRequest(context);

            Assert.Equal("user", Thread.CurrentPrincipal.Identity.Name);
            Assert.Equal("user", context.User.Identity.Name);
        }

        private string BuildAuthorizationHeader(string user, string password)
        {
            var header = user + ":" + password;
            var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(header);
            return "Basic " + Convert.ToBase64String(bytes);
        }

        private IConfiguration CreateMockConfiguration(string userName = "user", string password = "password")
        {
            var configuration = Substitute.For<IConfiguration>();
            configuration.GetAppSetting("basicAuthUser").Returns(userName);
            configuration.GetAppSetting("basicAuthPassword").Returns(password);

            return configuration;
        }

        private HttpContextBase CreateMockContext()
        {
            var context = Substitute.For<HttpContextBase>();
            var request = Substitute.For<HttpRequestBase>();
            var headers = new NameValueCollection();
            
            context.Request.Returns(request);
            request.Headers.Returns(headers);

            return context;
        }
    }
}
