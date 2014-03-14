using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;

namespace IronFoundry.ServiceBroker.Models
{
    public class MsSqlDatabaseBuilder : IDatabaseBuilder
    {
        private const string DatabaseNameFormatKey = "databaseNameFormat";
        private const string DatabaseUserFormatKey = "databaseUserFormat";
        private const string CloudFoundryConnectionStringKey = "CloudFoundry";
        private const string SqlPathKey = "sqlFilePath";
        private const string ServerPortKey = "sqlServerPort";
        private const string DataSourceKey = "sqlDataSource";

        private readonly string connectionString;
        private readonly string databaseFilePath;
        private readonly string databaseNameFormat;
        private readonly string databaseUserFormat;
        private readonly string dataSource;
        private readonly string serverPort;

        public MsSqlDatabaseBuilder(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString(CloudFoundryConnectionStringKey);
            databaseNameFormat = configuration.GetAppSetting(DatabaseNameFormatKey);
            databaseUserFormat = configuration.GetAppSetting(DatabaseUserFormatKey);
            databaseFilePath = Path.GetFullPath(configuration.GetAppSetting(SqlPathKey));
            dataSource = configuration.GetAppSetting(DataSourceKey);
            serverPort = configuration.GetAppSetting(ServerPortKey);
            if (!databaseFilePath.EndsWith(@"\"))
            {
                databaseFilePath = string.Format(@"{0}\", databaseFilePath);
            }
        }

        public void CreateDatabase(string databaseName, Plan plan)
        {
            var sqlPlan = plan as MsSqlPlan;
            Debug.Assert(sqlPlan != null, "plan should be typeof(MsSqlPlan)");
            
            var formattedDatabaseName = string.Format(databaseNameFormat, databaseName);

            if (!Directory.Exists(databaseFilePath))
            {
                throw new ArgumentException("The requested path for the data files does not exist. Please check your SQL file path in configuration.");
            }

            string createDatabaseCommand;
            if (sqlPlan.DatabaseSize >= 0)
            {
                createDatabaseCommand = string.Format(MsSqlTemplates.CreateLimitedDatabase, formattedDatabaseName, databaseFilePath, sqlPlan.DatabaseSize);
            }
            else
            {
                createDatabaseCommand = string.Format(MsSqlTemplates.CreateDatabase, formattedDatabaseName, databaseFilePath);
            }


            using (var sqlConn = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand(createDatabaseCommand, sqlConn))
                {
                    sqlConn.Open();
                    var codeResult = (int) cmd.ExecuteScalar();

                    if (codeResult == -1)
                    {
                        throw new InvalidOperationException(string.Format("The datbase {0} already exists.", databaseName));
                    }
                }
            }
        }

        public Credentials CreateBinding(string databaseName, string bindingId, Plan plan)
        {
            var sqlPlan = plan as MsSqlPlan;
            Debug.Assert(sqlPlan != null, "plan should be typeof(MsSqlPlan)");

            var formattedDatabaseName = string.Format(databaseNameFormat, databaseName);
            var userName = string.Format(databaseUserFormat, bindingId);
            var password = Guid.NewGuid().ToString();
            var command = string.Format(MsSqlTemplates.CreateUserForDatabase, formattedDatabaseName, userName, password); ;

            using (var sqlConn = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand(command, sqlConn))
                {
                    sqlConn.Open();
                    var codeResult = (int)cmd.ExecuteScalar();

                    if (codeResult == -1)
                    {
                        throw new InvalidOperationException(string.Format("The user {0} already exists for the database {1}.", userName, databaseName));
                    }
                }
            }

            return new MsSqlCredentials(userName, password, dataSource, serverPort, formattedDatabaseName);
        }

        public void RemoveBinding(string databaseName, string bindingId)
        {
            var formattedDatabaseName = string.Format(databaseNameFormat, databaseName);
            var userName = string.Format(databaseUserFormat, bindingId);
            var command = string.Format(MsSqlTemplates.DropUserFromDatabase, formattedDatabaseName, userName); ;

            using (var sqlConn = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand(command, sqlConn))
                {
                    sqlConn.Open();
                    var codeResult = (int)cmd.ExecuteScalar();

                    if (codeResult == -1)
                    {
                        throw new InvalidOperationException(string.Format("The user {0} does not exist for the database {1}.", userName, databaseName));
                    }
                }
            }
        }

        public void DropDatabase(string databaseName)
        {
            var formattedDatabaseName = string.Format(databaseNameFormat, databaseName);

            var query = string.Format(MsSqlTemplates.DropDatabase, formattedDatabaseName);

            using (var sqlConn = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand(query, sqlConn))
                {
                    sqlConn.Open();
                    var codeResult = (int) cmd.ExecuteScalar();

                    if (codeResult == -1)
                    {
                        throw new InvalidOperationException(string.Format("The database {0} does not exist.", databaseName));
                    }
                }
            }
        }
    }
}