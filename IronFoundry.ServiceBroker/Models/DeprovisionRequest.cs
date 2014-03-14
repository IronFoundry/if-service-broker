using Newtonsoft.Json;

namespace IronFoundry.ServiceBroker.Models
{
    public class DeprovisionRequest
    {
        [JsonIgnore]
        public string InstanceId { get; set; }

        [JsonProperty("service_id")]
        public string ServiceId { get; set; }

        [JsonProperty("plan_id")]
        public string PlanId { get; set; }
    }
}