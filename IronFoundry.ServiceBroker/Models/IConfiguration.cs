namespace IronFoundry.ServiceBroker.Models
{
    public interface IConfiguration
    {
        string GetAppSetting(string key);
        string GetConnectionString(string name);
    }
}