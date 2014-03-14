using System.Configuration;

namespace IronFoundry.ServiceBroker.Models
{
    internal class AppConfiguration : IConfiguration
    {
        public string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public string GetConnectionString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
    }
}