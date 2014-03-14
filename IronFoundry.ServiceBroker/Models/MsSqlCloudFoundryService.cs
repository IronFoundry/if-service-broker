using System;
using System.Linq;

namespace IronFoundry.ServiceBroker.Models
{
    public class MsSqlCloudFoundryService : ICloudFoundryService
    {
        private const string SqlDashboardKey = "sqlDashboardUrl";
        private const string LogUrlKey = "logUrl";

        private readonly IConfiguration configuration;
        private readonly IDatabaseBuilder databaseBuilder;
        
        public MsSqlCloudFoundryService(IConfiguration configuration, IDatabaseBuilder databaseBuilder)
        {
            this.configuration = configuration;
            this.databaseBuilder = databaseBuilder;
            Service = new MsSqlService();
            Id = Service.Id;
        }

        public string Id { get; private set; }

        public Service Service { get; private set; }

        public ProvisionResponse Provision(ProvisionRequest request)
        {
            string sqlDashboard = configuration.GetAppSetting(SqlDashboardKey);

            Plan plan = GetPlan(request.PlanId);

            databaseBuilder.CreateDatabase(request.InstanceId, plan);

            return new ProvisionResponse {Url = sqlDashboard};
        }

        public CreateBindingResponse CreateBinding(CreateBindingRequest request)
        {
            var logUrl = configuration.GetAppSetting(LogUrlKey);

            Plan plan = GetPlan(request.PlanId);

            Credentials credentials = databaseBuilder.CreateBinding(request.InstanceId, request.BindingId, plan);

            return new CreateBindingResponse {Credentials = credentials, LogUrl = logUrl};
        }

        public void RemoveBinding(RemoveBindingRequest request)
        {
            databaseBuilder.RemoveBinding(request.InstanceId, request.BindingId);
        }

        public void Deprovision(DeprovisionRequest request)
        {
            databaseBuilder.DropDatabase(request.InstanceId);
        }

        private Plan GetPlan(string planId)
        {
            var plan = Service.Plans.FirstOrDefault(x => x.Id.Equals(planId, StringComparison.InvariantCultureIgnoreCase)) as MsSqlPlan;
            if (plan == null)
            {
                throw new ArgumentException(string.Format("The plan id {0} does not exists", planId));
            }

            return plan;
        }
    }
}