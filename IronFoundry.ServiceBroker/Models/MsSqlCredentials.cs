using System.Data.SqlClient;

namespace IronFoundry.ServiceBroker.Models
{
    public class MsSqlCredentials : Credentials
    {
        public MsSqlCredentials(string user, string password, string server, string port, string database) : base(user, password, server, port, database)
        {
            var builder = new SqlConnectionStringBuilder();
            var dataSource = Server;
            if (!string.IsNullOrEmpty(Port))
            {
                dataSource = string.Format("{0},{1}", Server, Port);
            }
            builder.DataSource = dataSource;
            builder.InitialCatalog = Database;
            builder.UserID = User;
            builder.Password = Password;

            ConnctionString = builder.ConnectionString;
        }
    }
}