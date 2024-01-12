using apitude_meta.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using apitude_meta.Helper;
using StackExchange.Redis;
using DataProviders;
using Newtonsoft.Json.Bson;
using System.Reflection;

namespace apitude_meta.Common
{
    public class TypesRooms
    {
        public async Task<List<NewRoom>> GetDataFromSupplier()
        {
            List<NewRoom> roomList = new List<NewRoom>();

            MetaResponse response = await CallSupplier(1, 1000);
            if (response != null && response.rooms != null && response.rooms.Count > 0)
            {
                foreach (var x in response.rooms)
                {
                    roomList.Add(new NewRoom
                    {
                        code = x.code,
                        type = x.code.Split('.')[0],
                        characteristic = x.code.Split('.')[1],
                        minPax = x.minPax,
                        maxAdults = x.maxAdults,
                        maxChildren = x.maxChildren,
                        minAdults = x.minAdults,
                        description = x.description,
                        typeDescription = x.typeDescription.content,
                        characteristicDescription = x.characteristicDescription?.content ?? "",
                    });
                }
            }
            if (response != null && response.rooms != null && response.rooms.Count > 0)
            {
                while (response.to < response.total)
                {
                    response = await CallSupplier(response.to + 1, response.to + 1000);
                    foreach (var x in response.rooms)
                    {
                        roomList.Add(new NewRoom
                        {
                            code = x.code,
                            type = x.code.Split('.')[0],
                            characteristic = x.code.Split('.')[1],
                            minPax = x.minPax,
                            maxAdults = x.maxAdults,
                            maxChildren = x.maxChildren,
                            minAdults = x.minAdults,
                            description = x.description,
                            typeDescription = x.typeDescription.content,
                            characteristicDescription = x.characteristicDescription?.content ?? "",
                        });
                    }
                }
            }
            return roomList;
        }

        public async Task<MetaResponse> CallSupplier(int fromNumber, int toNumber)
        {
            var RoomsResponse = await Common.GetResponse(String.Empty, "/types/rooms?fields=all&language=ENG&from=" + fromNumber + "&to=" + toNumber + "&useSecondaryLanguage=True", "GET");
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            var response = JsonConvert.DeserializeObject<MetaResponse>(RoomsResponse.Body, jsonSerializerSettings);
            return response;
        }
        public async Task<dynamic> SaveToDatabase(List<NewRoom> Rooms)
        {
            DBConnect? obj = new DBConnect();
            {
                foreach (NewRoom item in Rooms)
                {
                    try
                    {
                        var existingData = await getExestingData(obj, item.code);
                        if (existingData == null)
                        {
                            obj.Insert("insert into room(code, type, characteristic, minPax, maxPax, maxAdults, maxChildren, minAdults, description, typeDescription, characteristicDescription) values('" + item.code + "', '" + item.type + "', '" + item.characteristic + "', '" + item.minPax + "', '" + item.maxPax + "', '" + item.maxAdults + "', '" + item.maxChildren + "', '" + item.minAdults + "', '" + item.description + "', '" + item.typeDescription + "', '" + item.characteristicDescription + "')");
                        }
                        else
                        {
                            obj.Insert("Update room set type = '" + item.type + "', " +
                                " type = '" + item.type + "', " +
                                " characteristic = '" + item.characteristic + "', " +
                                " minPax = '" + item.minPax + "', " +
                                " maxPax = '" + item.maxPax + "', " +
                                " maxAdults = '" + item.maxAdults + "', " +
                                " maxChildren = '" + item.maxChildren + "', " +
                                " minAdults = '" + item.minAdults + "', " +
                                " description = '" + item.description + "', " +
                                " typeDescription = '" + item.typeDescription + "', " +
                                " characteristicDescription = '" + item.characteristicDescription + "' " +
                                "where code = '" + item.code + "'");
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return "success";
        }

        private async Task<NewRoom> getExestingData(DBConnect dBConnect, string code)
        {
            List<NewRoom> list = new List<NewRoom>();
            MySqlDataReader dataReader = await dBConnect.Select("select * from room where code = '" + code + "'");
            while (dataReader.Read())
            {
                list.Add(new NewRoom()
                {
                    code = dataReader["code"].ToString(),
                    type = dataReader["type"].ToString(),
                    characteristic = dataReader["characteristic"].ToString(),
                    minPax = (int)dataReader["minPax"],
                    maxAdults = (int)dataReader["maxAdults"],
                    maxChildren = (int)dataReader["maxChildren"],
                    minAdults = (int)dataReader["minAdults"],
                    description = dataReader["description"].ToString(),
                    typeDescription = dataReader["typeDescription"].ToString(),
                    characteristicDescription = dataReader["characteristicDescription"].ToString(),
                });
            }

            dataReader.Close();

            dBConnect.CloseConnection();

            return list.FirstOrDefault();
        }
        public async void setToCache(List<NewRoom> rooms)
        {
            RedisProvider redisProvider = new RedisProvider();
            redisProvider.InitializeConnection(Environment.GetRedisConnectionString());
            List<HashEntry> redisHash = new List<HashEntry>();

            foreach (var item in rooms)
            {
                redisHash.Add(new HashEntry(item.code, item.Serialize()));
            }
            redisProvider.SetData("types:rooms", redisHash.ToArray());
        }
    }
    internal static class CassandraSerializationExtensions
    {
        private static readonly JsonSerializer Serializer = new JsonSerializer();

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] Serialize(this object obj)
        {
            if (obj == null) return null;

            using (var stream = new MemoryStream())
            {
                using (var bsonWriter = new BsonWriter(stream))
                {
                    Serializer.Serialize(bsonWriter, obj);
                }
                return stream.ToArray();
            }
        }
        public static Facility Deserialize(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            using (var reader = new BsonDataReader(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize<Facility>(reader);
            }
        }
    }
}
