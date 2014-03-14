using System.Collections.Generic;

namespace IronFoundry.ServiceBroker.Models
{
    public class Catalog
    {
        public Catalog() : this(new MsSqlDatabaseBuilder(new AppConfiguration()))
        {
            
        }

        public Catalog(IDatabaseBuilder databaseBuild)
        {
            Services = new Dictionary<string, ICloudFoundryService>();

            var sqlService = new MsSqlCloudFoundryService(new AppConfiguration(), databaseBuild);
            Services.Add(sqlService.Id, sqlService);
        }

        public IDictionary<string, ICloudFoundryService> Services { get; private set; }
    }
}