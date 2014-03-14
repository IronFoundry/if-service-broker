using System;
using IronFoundry.ServiceBroker.Models;
using NSubstitute;
using Xunit;

namespace IronFoundry.ServiceBroker.Tests
{
    public class MsSqlCloudFoundryServiceTests
    {
        public class Provision
        {
            [Fact]
            public void ReturnsCorrectUrlForDashboard()
            {
                var configuration = Substitute.For<IConfiguration>();
                var databaseBuilder = Substitute.For<IDatabaseBuilder>();
                configuration.GetAppSetting(Arg.Any<string>()).Returns("dashboard");
                var service = new MsSqlCloudFoundryService(configuration, databaseBuilder);
                var request = new ProvisionRequest { PlanId = MsSqlService.FreePlanId.ToString(), InstanceId = Guid.NewGuid().ToString() };

                ProvisionResponse response = service.Provision(request);

                Assert.Equal("dashboard", response.Url);
            }

            [Fact]
            public void CallsDatabaseBuilderWithCorrectPlan()
            {
                var configuration = Substitute.For<IConfiguration>();
                var databaseBuilder = Substitute.For<IDatabaseBuilder>();
                configuration.GetAppSetting(Arg.Any<string>()).Returns("dashboard");
                var service = new MsSqlCloudFoundryService(configuration, databaseBuilder);
                var request = new ProvisionRequest { PlanId = MsSqlService.FreePlanId.ToString(), InstanceId = Guid.NewGuid().ToString() };

                service.Provision(request);

                databaseBuilder.Received().CreateDatabase(Arg.Is<string>(x => x == request.InstanceId), Arg.Is<Plan>(x => x.Id == request.PlanId));
            }

            [Fact]
            public void ThrowsArgumentExceptionWhenPlanIdDoesNotExist()
            {
                var configuration = Substitute.For<IConfiguration>();
                var databaseBuilder = Substitute.For<IDatabaseBuilder>();
                configuration.GetAppSetting(Arg.Any<string>()).Returns("dashboard");
                var service = new MsSqlCloudFoundryService(configuration, databaseBuilder);
                var request = new ProvisionRequest { PlanId = Guid.NewGuid().ToString(), InstanceId = "instanceId"};

                var ex = Assert.Throws<ArgumentException>(() =>  service.Provision(request));

                Assert.Equal(string.Format("The plan id {0} does not exists", request.PlanId), ex.Message);
            }
        }

        public class CreateBinding
        {
            [Fact]
            public void ThrowsArgumentExceptionWhenPlanIdDoesNotExist()
            {
                var configuration = Substitute.For<IConfiguration>();
                var databaseBuilder = Substitute.For<IDatabaseBuilder>();
                configuration.GetAppSetting(Arg.Any<string>()).Returns("logurl");
                var service = new MsSqlCloudFoundryService(configuration, databaseBuilder);
                var request = new CreateBindingRequest { PlanId = Guid.NewGuid().ToString(), InstanceId = Guid.NewGuid().ToString(), BindingId = Guid.NewGuid().ToString() };

                var ex = Assert.Throws<ArgumentException>(() => service.CreateBinding(request));

                Assert.Equal(string.Format("The plan id {0} does not exists", request.PlanId), ex.Message);
            }

            [Fact]
            public void CallsDatabaseBuilderWithCorrectPlan()
            {
                var configuration = Substitute.For<IConfiguration>();
                var databaseBuilder = Substitute.For<IDatabaseBuilder>();
                configuration.GetAppSetting(Arg.Any<string>()).Returns("logurl");
                var service = new MsSqlCloudFoundryService(configuration, databaseBuilder);
                var request = new CreateBindingRequest { PlanId = MsSqlService.FreePlanId.ToString(), InstanceId = Guid.NewGuid().ToString(), BindingId = Guid.NewGuid().ToString() };

                service.CreateBinding(request);

                databaseBuilder.Received().CreateBinding(Arg.Is<string>(x => x == request.InstanceId), Arg.Is<string>(x => x == request.BindingId), Arg.Is<Plan>(x => x.Id == request.PlanId));
            }

            [Fact]
            public void ReturnsCredentialsForNewBinding()
            {
                var configuration = Substitute.For<IConfiguration>();
                var databaseBuilder = Substitute.For<IDatabaseBuilder>();
                databaseBuilder.CreateBinding(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Plan>()).ReturnsForAnyArgs(new Credentials());
                configuration.GetAppSetting(Arg.Any<string>()).Returns("logurl");
                var service = new MsSqlCloudFoundryService(configuration, databaseBuilder);
                var request = new CreateBindingRequest { PlanId = MsSqlService.FreePlanId.ToString(), InstanceId = Guid.NewGuid().ToString(), BindingId = Guid.NewGuid().ToString() };

                CreateBindingResponse response = service.CreateBinding(request);

                Assert.NotNull(response.Credentials);
            }

            [Fact]
            public void ReturnsCorrectUrlForSysLog()
            {
                var configuration = Substitute.For<IConfiguration>();
                var databaseBuilder = Substitute.For<IDatabaseBuilder>();
                databaseBuilder.CreateBinding(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Plan>()).ReturnsForAnyArgs(new Credentials());
                configuration.GetAppSetting(Arg.Any<string>()).Returns("logurl");
                var service = new MsSqlCloudFoundryService(configuration, databaseBuilder);
                var request = new CreateBindingRequest { PlanId = MsSqlService.FreePlanId.ToString(), InstanceId = Guid.NewGuid().ToString(), BindingId = Guid.NewGuid().ToString() };

                CreateBindingResponse response = service.CreateBinding(request);

                Assert.Equal("logurl", response.LogUrl);
            }
        }

        public class RemoveBinding
        {
            [Fact]
            public void CallsDatabaseBuilderWithCorrectArguments()
            {
                var configuration = Substitute.For<IConfiguration>();
                var databaseBuilder = Substitute.For<IDatabaseBuilder>();
                var service = new MsSqlCloudFoundryService(configuration, databaseBuilder);
                var request = new RemoveBindingRequest { InstanceId = Guid.NewGuid().ToString(), BindingId = Guid.NewGuid().ToString() };

                service.RemoveBinding(request);

                databaseBuilder.Received().RemoveBinding(Arg.Is<string>(x => x == request.InstanceId), Arg.Is<string>(x => x == request.BindingId));
            }
        }

        public class Deprovision
        {
            [Fact]
            public void CallsDatabaseBuilderToDropDatabase()
            {
                var configuration = Substitute.For<IConfiguration>();
                var databaseBuilder = Substitute.For<IDatabaseBuilder>();
                var service = new MsSqlCloudFoundryService(configuration, databaseBuilder);
                var request = new DeprovisionRequest { InstanceId = Guid.NewGuid().ToString() };

                service.Deprovision(request);

                databaseBuilder.Received().DropDatabase(Arg.Is<string>(x => x == request.InstanceId));
            }
        }
    }
}