using Newtonsoft.Json;

namespace IronFoundry.ServiceBroker.Models
{
    public class Credentials
    {
        public Credentials()
        {
        }

        public Credentials(string user, string password, string server, string port, string database)
        {
            User = user;
            Password = password;
            Server = server;
            Database = database;
            Port = port;
        }
        
        [JsonProperty("connection")]
        public string ConnctionString { get; set; }

        [JsonProperty("user")]
        public string User { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("server")]
        public string Server { get; set; }
        
        [JsonProperty("port")]
        public string Port { get; set; }
        
        [JsonProperty("database")]
        public string Database { get; set; }
    }
}