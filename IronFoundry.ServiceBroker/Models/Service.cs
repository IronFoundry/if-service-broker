using System.Collections.Generic;
using Newtonsoft.Json;

namespace IronFoundry.ServiceBroker.Models
{
    public class Service
    {
        public Service()
        {
            Plans = new List<Plan>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Bindable { get; set; }
        public ServiceMetadata Metadata { get; set; }
        public IList<Plan> Plans { get; private set; }
        public string[] Tags { get; set; }
        public string[] Requires { get; set; }
    }
}