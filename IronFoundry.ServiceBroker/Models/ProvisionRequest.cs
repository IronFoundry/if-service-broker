using Newtonsoft.Json;

namespace IronFoundry.ServiceBroker.Models
{
    public class ProvisionRequest
    {
        [JsonIgnore]
        public string InstanceId { get; set; }

        [JsonProperty("service_id")]
        public string ServiceId { get; set; }

        [JsonProperty("plan_id")]
        public string PlanId { get; set; }

        [JsonProperty("organization_guid")]
        public string Organization { get; set; }

        [JsonProperty("space_guid")]
        public string Space { get; set; }
    }
}