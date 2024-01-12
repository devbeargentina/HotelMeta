using apitude_meta.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using apitude_meta.Helper;
using DataProviders;
using StackExchange.Redis;

namespace apitude_meta.Common
{
    public class TypesPromotions
    {
        public async Task<List<NewPromotions>> GetDataFromSupplier()
        {
            var promotionsResponse = await Common.GetResponse(String.Empty, "/types/promotions?fields=all&language=ENG&from=1&to=100&useSecondaryLanguage=True", "GET");
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            var response = JsonConvert.DeserializeObject<MetaResponse>(promotionsResponse.Body, jsonSerializerSettings);
            if (response != null && response.promotions != null && response.promotions.Count > 0)
            {
                List<NewPromotions> responseData = response.promotions.Select(x => new NewPromotions { code = x.code, data = x.description?.content ?? x.name?.content }).ToList();

                return responseData;
            }
            return null;
        }
        public async Task<dynamic> SaveToDatabase(List<NewPromotions> promotions)
        {
            DBConnect? obj = new DBConnect();
            {
                foreach (NewPromotions item in promotions)
                {
                    var existingData = await getExestingData(obj, item.code);
                    if (existingData == null)
                    {
                        obj.Insert("insert into promotion(code, data) values('" + item.code + "', '" + item.data + "')");
                    }
                    else
                    {
                        obj.Insert("Update promotion set data = '" + item.data + "' where code = '" + item.code + "'");
                    }
                }
            }
            return "success";

        }

        private async Task<NewPromotions> getExestingData(DBConnect dBConnect, string code)
        {
            List<NewPromotions> list = new List<NewPromotions>();
            MySqlDataReader dataReader = await dBConnect.Select("select * from promotion where code = '" + code + "'");
            while (dataReader.Read())
            {
                list.Add(new NewPromotions()
                {
                    code = dataReader["code"].ToString(),
                    data = dataReader["data"].ToString()
                });
            }

            dataReader.Close();

            dBConnect.CloseConnection();

            return list.FirstOrDefault();
        }
        public async void setToCache(List<NewPromotions> promotions)
        {
            RedisProvider redisProvider = new RedisProvider();
            redisProvider.InitializeConnection(Environment.GetRedisConnectionString());
            List<HashEntry> redisHash = new List<HashEntry>();

            foreach (var item in promotions)
            {
                redisHash.Add(new HashEntry(item.code, item.data));
            }
            redisProvider.SetData("types:promotions", redisHash.ToArray());
        }
    }
}
