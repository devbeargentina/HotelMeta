using apitude_meta.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using apitude_meta.Helper;
using DataProviders;
using StackExchange.Redis;
using System.Xml.Serialization;
using System.Xml.Linq;
using MySqlX.XDevAPI.Common;
using System.Reflection.PortableExecutable;

namespace apitude_meta.Common
{
    public class LocationsZones
    {
        public async Task<List<Zones>> GetDataFromSupplier()
        {

            string request = @"
                            <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns=""http://www.juniper.es/webservice/2007/"">
                              <soapenv:Header/>
                              <soapenv:Body>
                                <ZoneList>
                                  <ZoneListRQ Version=""1.1"" Language=""en"">
                                   <Login Email=""##Username##"" Password=""##Password##""/>
                                    <ZoneListRequest ProductType=""HOT""/>
                                  </ZoneListRQ>
                                </ZoneList>
                              </soapenv:Body>
                            </soapenv:Envelope>
                            ";

            var ZonesResponse = await Common.GetResponse(request, "ZoneList", "POST");
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
 
            XDocument xmlDoc = XDocument.Parse(ZonesResponse.Body.ToString());

            // Find a specific element using LINQ to XML
            XNamespace soap = "http://schemas.xmlsoap.org/soap/envelope/";
            XNamespace juniper = "http://www.juniper.es/webservice/2007/";

            // Assuming you want to retrieve ZoneList element
            XElement zoneListElement = xmlDoc.Root?
                .Element(soap + "Body")?
                .Element(juniper + "ZoneListResponse")?
                .Element(juniper + "ZoneListRS")?
                .Element(juniper + "ZoneList");

            // Accessing data from the element, for example, iterating through Zone elements
            if (zoneListElement != null)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ZoneList));
                ZoneList result;

                using (System.IO.StringReader reader = new System.IO.StringReader(zoneListElement.ToString().Replace("xmlns=\"http://www.juniper.es/webservice/2007/\"","")))
                {
                    result = (ZoneList)serializer.Deserialize(reader);
                }

                if(result != null && result.Zones != null && result.Zones.Count > 0)
                {
                    List<Zones> responseData = result.Zones.Select(x => new Zones { JPDCode = x.JPDCode, ParentJPDCode = x.ParentJPDCode, AreaType = x.AreaType, Searchable = x.Searchable, Code = x.Code, ParentCode = x.ParentCode, Name = x.Name }).ToList();

                    return responseData;
                }

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
