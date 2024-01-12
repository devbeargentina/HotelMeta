using apitude_meta.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using apitude_meta.Helper;
using DataProviders;
using StackExchange.Redis;

namespace apitude_meta.Common
{
    public class LocationsDestination
    {
        public async Task<List<NewDestination>> GetDataFromSupplier()
        {
            List<NewDestination> responseData = new List<NewDestination>();
            MetaResponse response = await CallSupplier(1, 1000);
            RedisProvider redisProvider = new RedisProvider();
            redisProvider.InitializeConnection(Environment.GetRedisConnectionString());

            if (response != null && response.destinations != null && response.destinations.Count > 0)
            {
                foreach (var destination in response.destinations)
                {
                    foreach (var zone in destination.zones)
                    {
                        var CountryName = await redisProvider.GetData("locations:countries", destination.countryCode);
                        responseData.Add(new NewDestination()
                        {
                            DestinationCode = destination.code,
                            DestinationName = destination.name.content,
                            CountryCode = destination.countryCode,
                            CountryName = CountryName,
                            ZoneCode = zone.zoneCode,
                            ZoneName = zone.name,
                            DisplayName = destination.name.content + ", " + zone.name + ", " + CountryName
                        });
                    }
                }
            }

            if (response != null && response.destinations != null && response.destinations.Count > 0)
            {
                while (response.to < response.total)
                {
                    response = await CallSupplier(response.to + 1, response.to + 1000);
                    foreach (var destination in response.destinations)
                    {
                        foreach (var zone in destination.zones)
                        {
                            var CountryName = await redisProvider.GetData("locations:countries", destination.countryCode);
                            responseData.Add(new NewDestination()
                            {
                                DestinationCode = destination.code,
                                DestinationName = destination.name?.content?? zone.name??"",
                                CountryCode = destination.countryCode,
                                CountryName = CountryName,
                                ZoneCode = zone.zoneCode,
                                ZoneName = zone.name,
                                DisplayName = (destination.name?.content ?? zone.name ?? "") + ", " + zone.name + ", " + CountryName
                            });
                        }
                    }
                }
            }
            return responseData;
        }
        public async Task<MetaResponse> CallSupplier(int fromNumber, int toNumber)
        {
            var DestinationResponse = await Common.GetResponse(String.Empty, "/locations/destinations?fields=all&language=ENG&from=" + fromNumber + "&to=" + toNumber + "&useSecondaryLanguage=false", "GET");
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            var response = JsonConvert.DeserializeObject<MetaResponse>(DestinationResponse.Body, jsonSerializerSettings);

            return response;
        }

        public async Task<dynamic> SaveToDatabase(List<NewDestination> Destinations)
        {
            DBConnect? obj = new DBConnect();
            {
                foreach (NewDestination item in Destinations)
                {
                    try
                    {
                        var existingData = await getExestingData(obj, item);
                        if (existingData == null)
                        {

                            obj.Insert("insert into destination (destinationCode, destinationName, countryCode, countryName, zoneCode, zoneName, displayName) values('" + item.DestinationCode + "', '" + item.DestinationName.Replace("'", "''") + "', '" + item.CountryCode + "', '" + item.CountryName.Replace("'", "''") + "', '" + item.ZoneCode + "', '" + item.ZoneName.Replace("'", "''") + "', '" + item.DisplayName.Replace("'", "''") + "')");
                        }
                        else
                        {
                            obj.Insert("Update destination set destinationName = '" + item.DestinationName.Replace("'", "''") + "', countryName = '" + item.CountryName.Replace("'", "''") + "', zoneName = '" + item.ZoneName.Replace("'", "''") + "', displayName = '" + item.DisplayName.Replace("'", "''") + "' where destinationCode = '" + item.DestinationCode + "' AND CountryCode = '" + item.CountryCode + "' AND zoneCode = '" + item.ZoneCode + "'");
                        }
                    }
                    catch (Exception ex)
                    {
                        obj.CloseConnection();
                    }
                    //obj.Insert("delete from state where countrycode = '" + item.code + "'");
                }
            }
            return "success";

        }

        private async Task<NewDestination> getExestingData(DBConnect dBConnect, NewDestination destination)
        {
            List<NewDestination> list = new List<NewDestination>();

            try
            {
                MySqlDataReader dataReader = await dBConnect.Select("select * from destination where destinationCode = '" + destination.DestinationCode + "' AND countryCode = '" + destination.CountryCode + "' AND zoneCode = '" + destination.ZoneCode + "'");
                while (dataReader.Read())
                {
                    list.Add(new NewDestination()
                    {
                        DestinationCode = dataReader["DestinationCode"].ToString(),
                        DestinationName = dataReader["DestinationName"].ToString(),
                        CountryCode = dataReader["CountryCode"].ToString(),
                        CountryName = dataReader["CountryName"].ToString(),
                        ZoneCode = Convert.ToInt16(dataReader["ZoneCode"].ToString()),
                        ZoneName = dataReader["ZoneName"].ToString(),
                        DisplayName = dataReader["DisplayName"].ToString(),
                    });
                }

                dataReader.Close();

                dBConnect.CloseConnection();
            }
            catch (Exception ex)
            {

            }
            return list.FirstOrDefault();
        }
        public async void setToCache(List<NewDestination> Destinations)
        {
            RedisProvider redisProvider = new RedisProvider();
            redisProvider.InitializeConnection(Environment.GetRedisConnectionString());
            
            List<HashEntry> redisHashDestinations = new List<HashEntry>();

            foreach (var item in Destinations)
            {
                redisHashDestinations.Add(new HashEntry("destinationCode", item.DestinationCode));
                redisHashDestinations.Add(new HashEntry("destinationName", item.DestinationName));
                redisHashDestinations.Add(new HashEntry("countryCode", item.CountryCode));
                redisHashDestinations.Add(new HashEntry("countryName", item.CountryName));
                redisHashDestinations.Add(new HashEntry("zoneCode", item.ZoneCode));
                redisHashDestinations.Add(new HashEntry("zoneName", item.ZoneName));
                redisHashDestinations.Add(new HashEntry("displayName", item.DisplayName));

                redisProvider.SetData("destination:" + item.DestinationCode, redisHashDestinations.ToArray());

            }
        }
    }
}
