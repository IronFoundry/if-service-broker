using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Results;
using IronFoundry.ServiceBroker.Models;

namespace IronFoundry.ServiceBroker.Controllers
{
    public class CatalogController : ApiController
    {
        private readonly Catalog servicesCatalog;

        public CatalogController() : this(new Catalog())
        {
        }

        public CatalogController(Catalog servicesCatalog)
        {
            this.servicesCatalog = servicesCatalog;
        }

        [Route("v2/catalog", Name = "catalog")]
        public IHttpActionResult Get()
        {
            var response = new CatalogResponse();
            
            response.Services.AddRange(servicesCatalog.Services.Values.Select(x => x.Service));

            return Ok(response);
        }

        [Route("v2/service_instances/{instanceId}", Name = "provision")]
        [HttpPut]
        public IHttpActionResult Provision(string instanceId, [FromBody] ProvisionRequest request)
        {
            ICloudFoundryService service;
            if (!servicesCatalog.Services.TryGetValue(request.ServiceId, out service))
            {
                return BadRequest("This broker does not support the requested service.");
            }

            request.InstanceId = instanceId;

            try
            {
                ProvisionResponse response = service.Provision(request);
                return Content(HttpStatusCode.Created, response);
            }
            catch (InvalidOperationException)
            {
                return Conflict();
            }
        }

        // Had to hack the route so 
        [Route("service-broker/v2/service_instances/{instanceId}/service_bindings/{bindingId}", Name = "fixed-create_binding")]
        [Route("v2/service_instances/{instanceId}/service_bindings/{bindingId}", Name = "create_binding")]
        [HttpPut]
        public IHttpActionResult Bind(string instanceId, string bindingId, [FromBody] CreateBindingRequest request)
        {
            ICloudFoundryService service;
            if (!servicesCatalog.Services.TryGetValue(request.ServiceId, out service))
            {
                return BadRequest("This broker does not support the requested service.");
            }

            request.BindingId = bindingId;
            request.InstanceId = instanceId;

            try
            {
                CreateBindingResponse response = service.CreateBinding(request);
                return Ok(response);
            }
            catch (InvalidOperationException)
            {
                return Conflict();
            }
        }

        [Route("service-broker/v2/service_instances/{instanceId}/service_bindings/{bindingId}", Name = "fixed-remove_binding")]
        [Route("v2/service_instances/{instanceId}/service_bindings/{bindingId}", Name = "remove_binding")]
        [HttpDelete]
        public IHttpActionResult Unbind(string instanceId, string bindingId, [FromUri(Name = "service_id")] string serviceId, [FromUri(Name = "plan_id")]string planId)
        {
            ICloudFoundryService service;
            var request = new RemoveBindingRequest { ServiceId = serviceId, PlanId = planId, BindingId = bindingId, InstanceId = instanceId };

            if (request.ServiceId == null || !servicesCatalog.Services.TryGetValue(request.ServiceId, out service))
            {
                return BadRequest("This broker does not support the requested service.");
            }

            try
            {
                service.RemoveBinding(request);
                return Ok(new EmptyResponse());
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        [Route("service-broker/v2/service_instances/{instanceId}", Name = "fixed-deprovision")]
        [Route("v2/service_instances/{instanceId}", Name = "deprovision")]
        [HttpDelete]
        public IHttpActionResult Deprovision(string instanceId, [FromUri(Name = "service_id")] string serviceId, [FromUri(Name = "plan_id")]string planId)
        {
            ICloudFoundryService service;
            var request = new DeprovisionRequest { ServiceId = serviceId, PlanId = planId, InstanceId = instanceId };
            if (request.ServiceId == null || !servicesCatalog.Services.TryGetValue(request.ServiceId, out service))
            {
                return BadRequest("This broker does not support the requested service.");
            }

            try
            {
                service.Deprovision(request);
                return Ok(new EmptyResponse());
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }
    }
}