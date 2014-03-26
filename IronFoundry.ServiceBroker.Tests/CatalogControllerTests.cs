using System;
using System.Web.Http;
using System.Web.Http.Results;
using IronFoundry.ServiceBroker.Controllers;
using IronFoundry.ServiceBroker.Models;
using NSubstitute;
using Xunit;
using Xunit.Sdk;

namespace IronFoundry.ServiceBroker.Tests
{
    public class CatalogControllerTests
    {
        public class Get : WebApiTest
        {
            public Get()
            {
                Routes.MapHttpRoute("catalog", "v2/catalog");
            }

            [Fact]
            public void ReturnsCatalogOfServices()
            {
                var catalog = new Catalog(Substitute.For<IDatabaseBuilder>());
                var service = Substitute.For<ICloudFoundryService>();
                var serviceId = Guid.NewGuid();
                catalog.Services.Add(serviceId.ToString(), service);
                var controller = new CatalogController(catalog);

                IHttpActionResult result = controller.Get();
                var response = result as OkNegotiatedContentResult<CatalogResponse>;

                Assert.NotNull(response);
                Assert.Equal(2, response.Content.Services.Count);
            }
        }

        public class Provision : WebApiTest
        {
            private readonly ICloudFoundryService service;
            private Guid serviceId;
            private readonly CatalogController controller;

            public Provision()
            {
                Routes.MapHttpRoute("provision", "v2/service_instances/{instanceId}");
                var catalog = new Catalog(Substitute.For<IDatabaseBuilder>());
                service = Substitute.For<ICloudFoundryService>();
                serviceId = Guid.NewGuid();
                catalog.Services.Add(serviceId.ToString(), service);
                controller = new CatalogController(catalog);
            }


            [Fact]
            public void ReturnsBadRequestWhenServiceDoesnotExist()
            {
                IHttpActionResult result = controller.Provision("foo", new ProvisionRequest {ServiceId = "foo"});
                
                Assert.IsType<BadRequestErrorMessageResult>(result);
                Assert.Equal("This broker does not support the requested service.", ((BadRequestErrorMessageResult)result).Message);
            }

            [Fact]
            public void CallsServiceProvisionWithRequest()
            {
                service.Provision(Arg.Any<ProvisionRequest>()).ReturnsForAnyArgs(new ProvisionResponse { Url = "http://foo" });
                string instanceId = Guid.NewGuid().ToString();
                
                IHttpActionResult result = controller.Provision(instanceId, new ProvisionRequest { ServiceId = serviceId.ToString() });

                Assert.IsType<NegotiatedContentResult<ProvisionResponse>>(result);
                service.Received().Provision(Arg.Is<ProvisionRequest>(x => x.InstanceId == instanceId));
            }

            [Fact]
            public void ReturnsCreatedResponseEvenWhenUrlIsEmpty()
            {
                service.Provision(Arg.Any<ProvisionRequest>()).ReturnsForAnyArgs(new ProvisionResponse { Url = "" });
                string instanceId = Guid.NewGuid().ToString();

                IHttpActionResult result = controller.Provision(instanceId, new ProvisionRequest { ServiceId = serviceId.ToString() });

                Assert.IsType<NegotiatedContentResult<ProvisionResponse>>(result);
                service.Received().Provision(Arg.Is<ProvisionRequest>(x => x.InstanceId == instanceId));
            }

            [Fact]
            public void ReturnsResourceConflictWhenDatabaseAlreadyProvisioned()
            {
                service.When(x => x.Provision(Arg.Any<ProvisionRequest>())).Do(x => { throw new InvalidOperationException(); });

                IHttpActionResult result = controller.Provision(Guid.NewGuid().ToString(), new ProvisionRequest { ServiceId = serviceId.ToString() });

                Assert.IsType<ConflictResult>(result);
            }
        }

        public class Bind : WebApiTest
        {
            private readonly ICloudFoundryService service;
            private Guid serviceId;
            private readonly CatalogController controller;

            public Bind()
            {
                Routes.MapHttpRoute("create_binding", "v2/service_instances/{instanceId}/service_bindings/{bindingId}");
                var catalog = new Catalog(Substitute.For<IDatabaseBuilder>());
                service = Substitute.For<ICloudFoundryService>();
                serviceId = Guid.NewGuid();
                catalog.Services.Add(serviceId.ToString(), service);
                controller = new CatalogController(catalog);
            }

            [Fact]
            public void ReturnsBadRequestWhenServiceDoesnotExist()
            {
                IHttpActionResult result = controller.Bind("foo", "bar", new CreateBindingRequest { ServiceId = "foobar"});

                Assert.IsType<BadRequestErrorMessageResult>(result);
                Assert.Equal("This broker does not support the requested service.", ((BadRequestErrorMessageResult)result).Message);
            }

            [Fact]
            public void CallsServiceCreateBindingAfterSettingInstanceIdAndBindingIdOnRequest()
            {
                service.CreateBinding(Arg.Any<CreateBindingRequest>()).ReturnsForAnyArgs(new CreateBindingResponse { LogUrl = "http://foo", Credentials = new Credentials() });
                var request = new CreateBindingRequest { ServiceId = serviceId.ToString() };
                var instanceId = Guid.NewGuid().ToString();
                var bindingId = Guid.NewGuid().ToString();
               
                IHttpActionResult result = controller.Bind(instanceId, bindingId, request);

                Assert.IsType<OkNegotiatedContentResult<CreateBindingResponse>>(result);
                service.Received().CreateBinding(Arg.Is<CreateBindingRequest>(x => x.BindingId == bindingId && x.InstanceId == instanceId));
            }

            [Fact]
            public void ReturnsResourceConflictWhenUserAlreadyProvisioned()
            {
                service.When(x => x.CreateBinding(Arg.Any<CreateBindingRequest>())).Do(x => { throw new InvalidOperationException(); });

                IHttpActionResult result = controller.Bind(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), new CreateBindingRequest { ServiceId = serviceId.ToString() });

                Assert.IsType<ConflictResult>(result);
            }
        }

        public class Unbind : WebApiTest
        {
            private readonly ICloudFoundryService service;
            private Guid serviceId;
            private readonly CatalogController controller;

            public Unbind()
            {
                Routes.MapHttpRoute("create_binding", "v2/service_instances/{instanceId}/service_bindings/{bindingId}");
                var catalog = new Catalog(Substitute.For<IDatabaseBuilder>());
                service = Substitute.For<ICloudFoundryService>();
                serviceId = Guid.NewGuid();
                catalog.Services.Add(serviceId.ToString(), service);
                controller = new CatalogController(catalog);
            }

            [Fact]
            public void ReturnsBadRequestWhenServiceDoesnotExist()
            {
                IHttpActionResult result = controller.Unbind("instanceId", "bindingId", "serviceId", "planId");

                Assert.IsType<BadRequestErrorMessageResult>(result);
                Assert.Equal("This broker does not support the requested service.", ((BadRequestErrorMessageResult)result).Message);
            }

            [Fact]
            public void ReturnsBadRequestWhenServiceIdIsNull()
            {
                IHttpActionResult result = controller.Unbind("instanceId", "bindingId", null, "planId");

                Assert.IsType<BadRequestErrorMessageResult>(result);
                Assert.Equal("This broker does not support the requested service.", ((BadRequestErrorMessageResult)result).Message);
            }

            [Fact]
            public void CallsServiceCreateBindingAfterSettingInstanceIdAndBindingIdOnRequest()
            {
                var instanceId = Guid.NewGuid().ToString();
                var bindingId = Guid.NewGuid().ToString();
                var planId = Guid.NewGuid().ToString();

                IHttpActionResult result = controller.Unbind(instanceId, bindingId, serviceId.ToString(), planId);
                
                Assert.IsType<OkNegotiatedContentResult<EmptyResponse>>(result);
                service.Received().RemoveBinding(Arg.Is<RemoveBindingRequest>(x => x.BindingId == bindingId && x.InstanceId == instanceId && x.ServiceId == serviceId.ToString() && x.PlanId == planId));
            }

            [Fact]
            public void ReturnsResourceConflictWhenUserAlreadyProvisioned()
            {
                service.When(x => x.RemoveBinding(Arg.Any<RemoveBindingRequest>())).Do(x => { throw new InvalidOperationException(); });

                IHttpActionResult result = controller.Unbind(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), serviceId.ToString(), Guid.NewGuid().ToString());

                Assert.IsType<NotFoundResult>(result);
            }
        }

        public class Deprovision : WebApiTest
        {
            private readonly ICloudFoundryService service;
            private Guid serviceId;
            private readonly CatalogController controller;

            public Deprovision()
            {
                Routes.MapHttpRoute("deprovision", "v2/service_instances/{instanceId}");
                var catalog = new Catalog(Substitute.For<IDatabaseBuilder>());
                service = Substitute.For<ICloudFoundryService>();
                serviceId = Guid.NewGuid();
                catalog.Services.Add(serviceId.ToString(), service);
                controller = new CatalogController(catalog);
            }

            [Fact]
            public void ReturnsBadRequestWhenServiceDoesnotExist()
            {
                IHttpActionResult result = controller.Deprovision("foo", "foo", "planId");
                
                Assert.IsType<BadRequestErrorMessageResult>(result);
                Assert.Equal("This broker does not support the requested service.", ((BadRequestErrorMessageResult)result).Message);
            }

            [Fact]
            public void ReturnsBadRequestWhenServiceIdIsNull()
            {
                IHttpActionResult result = controller.Deprovision("foo", null, "planId");

                Assert.IsType<BadRequestErrorMessageResult>(result);
                Assert.Equal("This broker does not support the requested service.", ((BadRequestErrorMessageResult)result).Message);
            }

            [Fact]
            public void CallsServiceDeprovisionWithInstanceId()
            {
                var instanceId = Guid.NewGuid().ToString();
                var planId = Guid.NewGuid().ToString();
                
                IHttpActionResult result = controller.Deprovision(instanceId, serviceId.ToString(), planId);

                Assert.IsType<OkNegotiatedContentResult<EmptyResponse>>(result);
                service.Received().Deprovision(Arg.Is<DeprovisionRequest>(x => x.InstanceId == instanceId && x.ServiceId == serviceId.ToString() && x.PlanId == planId));
            }

            [Fact]
            public void ReturnsNotFoundWhenInstanceDoesNotExist()
            {
                service.When(x => x.Deprovision(Arg.Any<DeprovisionRequest>())).Do(x => { throw new InvalidOperationException(); });

                IHttpActionResult result = controller.Deprovision(Guid.NewGuid().ToString(), serviceId.ToString(), Guid.NewGuid().ToString());

                Assert.IsType<NotFoundResult>(result);
            }
        }
    }
}