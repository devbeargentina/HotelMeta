using apitude_meta.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using apitude_meta.Helper;
using DataProviders;
using StackExchange.Redis;

namespace apitude_meta.Common
{
    public class LocationsCountries
    {
        public async Task<List<NewCountry>> GetDataFromSupplier()
        {
            var CountriesResponse = await Common.GetResponse(String.Empty, "/locations/countries?fields=all&language=ENG&from=1&to=1000", "GET");
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            var response = JsonConvert.DeserializeObject<MetaResponse>(CountriesResponse.Body, jsonSerializerSettings);
            if (response != null && response.countries != null && response.countries.Count > 0)
            {
                List<NewCountry> responseData = response.countries.Select(x => new NewCountry { code = x.code, name = x.description.content, states = x.states }).ToList();

                return responseData;
            }
            return null;
        }
        public async Task<dynamic> SaveToDatabase(List<NewCountry> countries)
        {
            DBConnect? obj = new DBConnect();
            {
                foreach (NewCountry item in countries)
                {
                    var existingData = await getExestingData(obj, item.code);
                    if (existingData == null)
                    {
                        try
                        {
                            obj.Insert("insert into country (code, name) values('" + item.code + "', '" + item.name + "')");
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    else
                    {
                        obj.Insert("Update country set name = '" + item.name + "' where code = '" + item.code + "'");
                    }
                    obj.Insert("delete from state where countrycode = '" + item.code + "'");
                    if (item.states.Count > 0)
                    {
                        foreach (var state in item.states.Where(x=>x.code!="07" && x.code != "XX"))
                        {
                            try
                            {
                                obj.Insert("insert into state (code, name, countrycode) values('" + state.code + "', '" + state.name.Replace("'", "''") + "', '" + item.code + "')");

                            }
                            catch (Exception ex)
                            {

                            }
                            finally
                            {
                                obj.CloseConnection();
                            }
                        }
                    }
                }
            }
            return "success";

        }

        private async Task<NewCountry> getExestingData(DBConnect dBConnect, string code)
        {
            List<NewCountry> list = new List<NewCountry>();
            try
            {
                MySqlDataReader dataReader = await dBConnect.Select("select * from country where code = '" + code + "'");
                while (dataReader.Read())
                {
                    list.Add(new NewCountry()
                    {
                        code = dataReader["code"].ToString(),
                        name = dataReader["name"].ToString(),
                        states = null
                    });
                }
                dataReader.Close();

                dBConnect.CloseConnection();
                List<State> states = new List<State>();
                try
                {
                    MySqlDataReader dataReaderState = await dBConnect.Select("select * from state where countrycode = '" + code + "'");
                    while (dataReaderState.Read())
                    {
                        states.Add(new State()
                        {
                            code = dataReaderState["code"].ToString(),
                            name = dataReaderState["name"].ToString()
                        });
                    }
                }
                catch (Exception ex)
                {

                }
                dataReader.Close();

                dBConnect.CloseConnection();
                if (list != null && list.Count > 0 && states.Count > 0)
                    list.FirstOrDefault().states = states;
            }
            catch (Exception ex)
            {

            }
            return list.FirstOrDefault();
        }
        public async void setToCache(List<NewCountry> countries)
        {
            RedisProvider redisProvider = new RedisProvider();
            redisProvider.InitializeConnection(Environment.GetRedisConnectionString());
            List<HashEntry> redisHashCountries = new List<HashEntry>();
            List<HashEntry> redisHashStates = new List<HashEntry>();

            foreach (var item in countries)
            {
                redisHashCountries.Add(new HashEntry(item.code, item.name));

                foreach (var state in item.states)
                {
                    redisHashStates.Add(new HashEntry(state.code, state.name));
                }
                redisProvider.SetData("locations:states:" + item.code, redisHashStates.ToArray());

            }
            redisProvider.SetData("locations:countries", redisHashCountries.ToArray());

        }
    }
}
