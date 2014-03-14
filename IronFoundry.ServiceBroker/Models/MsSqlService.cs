using System;

namespace IronFoundry.ServiceBroker.Models
{
    public class MsSqlService : Service
    {
        public static readonly Guid ServiceId = new Guid("7AD5A5F8-D13E-42BF-A22D-A7B0C4E6A06D");
        public static readonly Guid FreePlanId = new Guid("69D2FFD5-0902-46F5-A603-E56354E430EF");

        public MsSqlService()
        {
            Id = ServiceId.ToString();
            Name = "ms-sql";
            Description = "Microsoft SQL Server Service";
            Bindable = true;
            Requires = new string[0];
            Tags = new string[0];
            Metadata = new ServiceMetadata();

            Plans.Add(new MsSqlPlan
                      {
                          Id = FreePlanId.ToString(),
                          Name = "Free",
                          Description = "Free plan",
                          DatabaseSize = 1024,
                          Metadata = new PlanMetadata
                                     {
                                         DisplayName = "1 gig",
                                         Bullets = new[] {"1 gig database size"},
                                         Costs = new[] {new Cost {Unit = "Monthly", Amount = new Amount {Denomination = "USD", Value = 0}}}
                                     }
                      });
        }
    }
}