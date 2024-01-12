using apitude_meta.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using apitude_meta.Helper;
using DataProviders;
using Newtonsoft.Json.Bson;
using System.Reflection;
using StackExchange.Redis;

namespace apitude_meta.Common
{
    public class TypesAccommodations
    {
        public async Task<List<NewAccommodation>> GetDataFromSupplier()
        {
            var AccommodationsResponse = await Common.GetResponse(String.Empty, "/types/accommodations?fields=all&language=ENG&from=1&to=1000&useSecondaryLanguage=True", "GET");
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            var response = JsonConvert.DeserializeObject<MetaResponse>(AccommodationsResponse.Body, jsonSerializerSettings);
            if (response != null && response.accommodations != null && response.accommodations.Count > 0)
            {
                List<NewAccommodation> responseData = response.accommodations.Select(x => new NewAccommodation { code = x.code, data = x.typeDescription }).ToList();

                return responseData;
            }
            return null;
        }
        public async Task<dynamic> SaveToDatabase(List<NewAccommodation> Accommodations)
        {
            DBConnect? obj = new DBConnect();
            {
                foreach (NewAccommodation item in Accommodations)
                {
                    var existingData = await getExestingData(obj, item.code);
                    if (existingData == null)
                    {
                        obj.Insert("insert into accommodation(code, data) values('" + item.code + "', '" + item.data + "')");
                    }
                    else
                    {
                        obj.Insert("Update accommodation set data = '" + item.data + "' where code = '" + item.code + "'");
                    }
                }
            }
            return "success";

        }

        private async Task<NewAccommodation> getExestingData(DBConnect dBConnect, string code)
        {
            List<NewAccommodation> list = new List<NewAccommodation>();
            MySqlDataReader dataReader = await dBConnect.Select("select * from accommodation where code = '" + code + "'");
            while (dataReader.Read())
            {
                list.Add(new NewAccommodation()
                {
                    code = dataReader["code"].ToString(),
                    data = dataReader["data"].ToString()
                });
            }

            dataReader.Close();

            dBConnect.CloseConnection();

            return list.FirstOrDefault();
        }
        public async void setToCache(List<NewAccommodation> accommodation)
        {
            RedisProvider redisProvider = new RedisProvider();
            redisProvider.InitializeConnection(Environment.GetRedisConnectionString());
            List<HashEntry> redisHash = new List<HashEntry>();

            foreach (var item in accommodation)
            {
                redisHash.Add(new HashEntry(item.code, item.data));
            }
            redisProvider.SetData("types:accommodation", redisHash.ToArray());
        }
    }
}