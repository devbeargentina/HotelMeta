using DataProviders.Interface;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Reflection;

namespace DataProviders
{
    public class RedisProvider
    {
        RedisConnection _RedisConnection;
        StackExchange.Redis.IDatabase _Redisdb;

        const string Prefix = "APITude:";
        public void InitializeConnection(string connectionString)
        {
            _RedisConnection = new RedisConnection(connectionString);
            _Redisdb = _RedisConnection.Connection.GetDatabase();
        }

        public void SetData(string Key, HashEntry[] HashValues)
        {
            //http://taswar.zeytinsoft.com/redis-hash-datatype/
            _Redisdb.HashSetAsync(Key, HashValues);
        }

        public async Task<dynamic> GetData(string Key, string HashKey, bool isByteData = false)
        {
            if (isByteData)
            {
                var output = await _Redisdb.HashGetAsync(Prefix + Key, HashKey);
                return (byte[])output;
            }
            else
                return await _Redisdb.HashGetAsync(Prefix + Key, HashKey);
        }

        public async Task<HashEntry[]> GetDataAll(string HashKey)
        {
            return await _Redisdb.HashGetAllAsync(Prefix + HashKey);
        }

        public string GetData(string Key)
        {
            return _Redisdb.StringGet(Prefix + Key);
        }

        public async Task<bool> SetData(string Key, string value)
        {
            return await _Redisdb.StringSetAsync(Prefix + Key, value);
        }
        public HashEntry[] ToHashEntries(object obj)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();
            return properties
                .Where(x => x.GetValue(obj) != null) // <-- PREVENT NullReferenceException
                .Select
                (
                      property =>
                      {
                          object propertyValue = property.GetValue(obj);
                          string hashValue;

                          // This will detect if given property value is 
                          // enumerable, which is a good reason to serialize it
                          // as JSON!
                          if (propertyValue is IEnumerable<object>)
                          {
                              // So you use JSON.NET to serialize the property
                              // value as JSON
                              hashValue = JsonConvert.SerializeObject(propertyValue);
                          }
                          else
                          {
                              hashValue = propertyValue.ToString();
                          }

                          return new HashEntry(property.Name, hashValue);
                      }
                )
                .ToArray();
        }

        public T ConvertFromRedis<T>(HashEntry[] hashEntries)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            var obj = Activator.CreateInstance(typeof(T));
            foreach (var property in properties)
            {
                HashEntry entry = hashEntries.FirstOrDefault(g => g.Name.ToString().Equals(property.Name));
                if (entry.Equals(new HashEntry())) continue;
                property.SetValue(obj, Convert.ChangeType(entry.Value.ToString(), property.PropertyType));
            }
            return (T)obj;
        }
    }
}