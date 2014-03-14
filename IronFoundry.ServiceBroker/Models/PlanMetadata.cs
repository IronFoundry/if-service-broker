using System.Linq;

namespace IronFoundry.ServiceBroker.Models
{
    public class PlanMetadata
    {
        public string[] Bullets { get; set; }
        public Cost[] Costs { get; set; }
        public string DisplayName { get; set; }
    }
}