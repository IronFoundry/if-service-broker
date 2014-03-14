namespace IronFoundry.ServiceBroker.Models
{
    public class Plan
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public PlanMetadata Metadata { get; set; }
    }
}