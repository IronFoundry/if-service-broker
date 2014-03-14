using System.Collections.Generic;
using Newtonsoft.Json.Serialization;

namespace IronFoundry.ServiceBroker.Models
{
    public class CatalogResponse
    {
        public CatalogResponse()
        {
            Services = new List<Service>();
        }

        public List<Service> Services { get; private set; }
    }
}