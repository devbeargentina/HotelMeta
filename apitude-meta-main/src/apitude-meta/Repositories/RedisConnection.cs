using StackExchange.Redis;
using System.Collections.Generic;

namespace DataProviders.Interface
{
    public class RedisConnection
    {
        private static Dictionary<string, string> cacheConfig;

        public RedisConnection(string connectionstring)
        {
            cacheConfig = new Dictionary<string, string>();
            LoadRedisConfig(connectionstring);

            var config = new ConfigurationOptions()
            {
                EndPoints =
                {
                    { cacheConfig["HOST"], int.Parse(cacheConfig["PORT"]) }
                }
            };

            if (cacheConfig["MODE"] == "ssdb")
            {
                config.CommandMap = CommandMap.SSDB;
            }

            if (cacheConfig["MODE"] == "twemproxy")
            {
                config.CommandMap = CommandMap.Twemproxy;
            }
            config.AbortOnConnectFail = false;
            Connection = ConnectionMultiplexer.Connect(config);
            Connection.PreserveAsyncOrder = false;
        }

        public ConnectionMultiplexer Connection;

        private static void LoadRedisConfig(string Config)
        {
            string[] connectionString = Config.Split(new char[] { ',' });
            string ipAddress = connectionString[0];
            string portNumber = connectionString[1];
            string mode = connectionString[2];

            cacheConfig.Add("HOST", ipAddress);
            cacheConfig.Add("PORT", portNumber);
            cacheConfig.Add("MODE", mode);
        }

    }

}