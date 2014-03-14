using Newtonsoft.Json;

namespace IronFoundry.ServiceBroker.Models
{
    public class ProvisionResponse
    {
        [JsonProperty("dashboard_url")]
        public string Url { get; set; }
    }
}