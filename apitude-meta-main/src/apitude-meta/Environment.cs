using MySql.Data.MySqlClient;
using System.Data;

namespace apitude_meta
{
    public class Environment
    {
        public static string GetSharedServiceUrl()
        {
            return System.Environment.GetEnvironmentVariable("SharedServiceUrl");
        }
        public static string GetApiKey()
        {
            return System.Environment.GetEnvironmentVariable("ApiKey");
        }
        public static string GetApiSecreat()
        {
            return System.Environment.GetEnvironmentVariable("ApiSecreat");
        }
        public static string GetMysqlConnection()
        {
            return System.Environment.GetEnvironmentVariable("mysql-connectionstring");
        }
        public static IDbConnection GetConnection()
        {
            return new MySqlConnection(GetMysqlConnection()); 
        }
        public static string GetRedisConnectionString()
        {
            return System.Environment.GetEnvironmentVariable("RedisConnectionString");
        }
        public static string GetApiUsername()
        {
            return System.Environment.GetEnvironmentVariable("ApiUsername");
        }
        public static string GetAPiPassword()
        {
            return System.Environment.GetEnvironmentVariable("ApiPassword");
        }
    }
}