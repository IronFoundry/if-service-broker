using Newtonsoft.Json;

namespace IronFoundry.ServiceBroker.Models
{
    public class CreateBindingResponse
    {
        [JsonProperty("credentials")]
        public object Credentials { get; set; }

        [JsonProperty("syslog_drain_url")]
        public string LogUrl { get; set; }
    }
}