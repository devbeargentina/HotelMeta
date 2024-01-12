using apitude_meta.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using apitude_meta.Helper;
using DataProviders;
using StackExchange.Redis;

namespace apitude_meta.Common
{
    public class TypesRateClass
    {
        public async Task<List<NewRateClass>> GetDataFromSupplier()
        {
            var RateClasssResponse = await Common.GetResponse(String.Empty, "/types/classifications", "GET");
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            var response = JsonConvert.DeserializeObject<MetaResponse>(RateClasssResponse.Body, jsonSerializerSettings);
            if (response != null && response.classifications != null && response.classifications.Count > 0)
            {
                List<NewRateClass> responseData = response.classifications.Select(x => new NewRateClass { code = x.code, data = x.description.content}).ToList();

                return responseData;
            }
            return null;
        }
        public async Task<dynamic> SaveToDatabase(List<NewRateClass> RateClasss)
        {
            DBConnect? obj = new DBConnect();
            {
                foreach (NewRateClass item in RateClasss)
                {
                    var existingData = await getExestingData(obj, item.code);
                    if (existingData == null)
                    {
                        obj.Insert("insert into rateclass (code, data) values('" + item.code + "', '" + item.data + "')");
                    }
                    else
                    {
                        obj.Insert("Update rateclass set data = '" + item.data + "' where code = '" + item.code + "'");
                    }
                }
            }
            return "success";

        }

        private async Task<NewRateClass> getExestingData(DBConnect dBConnect, string code)
        {
            List<NewRateClass> list = new List<NewRateClass>();
            MySqlDataReader dataReader = await dBConnect.Select("select * from rateclass where code = '" + code + "'");
            while (dataReader.Read())
            {
                list.Add(new NewRateClass()
                {
                    code = dataReader["code"].ToString(),
                    data = dataReader["data"].ToString()
                });
            }

            dataReader.Close();

            dBConnect.CloseConnection();

            return list.FirstOrDefault();
        }
        public async void setToCache(List<NewRateClass> rateClasses)
        {
            RedisProvider redisProvider = new RedisProvider();
            redisProvider.InitializeConnection(Environment.GetRedisConnectionString());
            List<HashEntry> redisHash = new List<HashEntry>();

            foreach (var item in rateClasses)
            {
                redisHash.Add(new HashEntry(item.code, item.data));
            }
            redisProvider.SetData("types:rateclass", redisHash.ToArray());
        }
    }
}
