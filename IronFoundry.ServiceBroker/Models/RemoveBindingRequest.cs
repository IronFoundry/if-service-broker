using Newtonsoft.Json;

namespace IronFoundry.ServiceBroker.Models
{
    public class RemoveBindingRequest
    {
        [JsonIgnore]
        public string InstanceId { get; set; }

        [JsonIgnore]
        public string BindingId { get; set; }

        [JsonProperty("service_id")]
        public string ServiceId { get; set; }

        [JsonProperty("plan_id")]
        public string PlanId { get; set; }
    }
}