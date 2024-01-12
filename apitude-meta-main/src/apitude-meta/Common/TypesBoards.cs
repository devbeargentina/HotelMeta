using apitude_meta.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using apitude_meta.Helper;
using DataProviders;
using StackExchange.Redis;

namespace apitude_meta.Common
{
    public class TypesBoards
    {
        public async Task<List<NewBoard>> GetDataFromSupplier()
        {
            var BoardsResponse = await Common.GetResponse(String.Empty, "/types/boards?fields=all&language=ENG&from=1&to=100&useSecondaryLanguage=True", "GET");
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            var response = JsonConvert.DeserializeObject<MetaResponse>(BoardsResponse.Body, jsonSerializerSettings);
            if (response != null && response.boards != null && response.boards.Count > 0)
            {
                List<NewBoard> responseData = response.boards.Select(x => new NewBoard { code = x.code, data = x.description.content}).ToList();

                return responseData;
            }
            return null;
        }
        public async Task<dynamic> SaveToDatabase(List<NewBoard> Boards)
        {
            DBConnect? obj = new DBConnect();
            {
                foreach (NewBoard item in Boards)
                {
                    var existingData = await getExestingData(obj, item.code);
                    if (existingData == null)
                    {
                        obj.Insert("insert into boardclass (code, data) values('" + item.code + "', '" + item.data + "')");
                    }
                    else
                    {
                        obj.Insert("Update boardclass set data = '" + item.data + "' where code = '" + item.code + "'");
                    }
                }
            }
            return "success";

        }

        private async Task<NewBoard> getExestingData(DBConnect dBConnect, string code)
        {
            List<NewBoard> list = new List<NewBoard>();
            MySqlDataReader dataReader = await dBConnect.Select("select * from boardclass where code = '" + code + "'");
            while (dataReader.Read())
            {
                list.Add(new NewBoard()
                {
                    code = dataReader["code"].ToString(),
                    data = dataReader["data"].ToString()
                });
            }

            dataReader.Close();

            dBConnect.CloseConnection();

            return list.FirstOrDefault();
        }
        public async void setToCache(List<NewBoard> board)
        {
            RedisProvider redisProvider = new RedisProvider();
            redisProvider.InitializeConnection(Environment.GetRedisConnectionString());
            List<HashEntry> redisHash = new List<HashEntry>();

            foreach (var item in board)
            {
                redisHash.Add(new HashEntry(item.code, item.data));
            }
            redisProvider.SetData("types:board", redisHash.ToArray());
        }
    }
}
