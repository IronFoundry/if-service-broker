using System.Data;

namespace IronFoundry.ServiceBroker.Models
{
    public interface IDatabaseBuilder
    {
        void CreateDatabase(string databaseName, Plan plan);
        Credentials CreateBinding(string databaseName, string bindingId, Plan plan);
        void RemoveBinding(string databaseName, string bindingId);
        void DropDatabase(string databaseName);
    }
}