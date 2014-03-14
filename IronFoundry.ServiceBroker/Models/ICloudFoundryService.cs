namespace IronFoundry.ServiceBroker.Models
{
    public interface ICloudFoundryService
    {
        string Id { get; }
        Service Service { get; }
        ProvisionResponse Provision(ProvisionRequest request);
        CreateBindingResponse CreateBinding(CreateBindingRequest request);
        void RemoveBinding(RemoveBindingRequest request);
        void Deprovision(DeprovisionRequest request);
    }
}