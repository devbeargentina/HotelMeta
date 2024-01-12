using apitude_meta.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using apitude_meta.Helper;
using StackExchange.Redis;
using DataProviders;
using Newtonsoft.Json.Bson;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json.Linq;
using MongoDB.Driver;
using MongoDB.Bson;

namespace apitude_meta.Common
{
    public class HotelsJuniper
    {

        public async Task<List<Hotel>> GetDataFromSupplierJuniper()
        {

            string request = @"
                            <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns=""http://www.juniper.es/webservice/2007/"">
                              <soapenv:Header/>
                              <soapenv:Body>
                                <HotelPortfolio>
                                  <HotelPortfolioRQ Version=""1.1"" Language=""en"" RecordsPerPage=""500"">
                                    <Login Email=""##Username##"" Password=""##Password##""/>
                                  </HotelPortfolioRQ>
                                </HotelPortfolio>
                              </soapenv:Body>
                            </soapenv:Envelope>
                            ";

            var HotelsResponse = await Common.GetResponse(request, "HotelPortfolio", "POST");
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            XDocument xmlDoc = XDocument.Parse(HotelsResponse.Body.ToString());

            // Find a specific element using LINQ to XML
            XNamespace soap = "http://schemas.xmlsoap.org/soap/envelope/";
            XNamespace juniper = "http://www.juniper.es/webservice/2007/";

            // Assuming you want to retrieve hotellist element
            XElement HotelListElement = xmlDoc.Root?
                .Element(soap + "Body")?
                .Element(juniper + "HotelPortfolioResponse")?
                .Element(juniper + "HotelPortfolioRS")?
                .Element(juniper + "HotelPortfolio");

            // Accessing data from the element, for example, iterating through Zone elements
            if (HotelListElement != null)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(HotelPortfolio));
                HotelPortfolio result;

                using (System.IO.StringReader reader = new System.IO.StringReader(HotelListElement.ToString().Replace("xmlns=\"http://www.juniper.es/webservice/2007/\"", "")))
                {
                    result = (HotelPortfolio)serializer.Deserialize(reader);
                }

                if (result != null && result.Hotel != null && result.Hotel.Count > 0)
                {
                    gethotelcontent(result.Hotel);
                    return null;
                }

            }



            return null;
        }

        public async void gethotelcontent(List<HotelInfoPortfolio> hotels)
        {
            try
            {
                //var connectionString = "mongodb://Chetan%4011Nov2023:password@argentinamongodb.mongocluster.cosmos.azure.com/Argentina";
                //var client = new MongoClient(connectionString);
                var client = new MongoClient("mongodb://Chetan%4011Nov2023:password@argentinamongodb.mongocluster.cosmos.azure.com/?tls=true&authMechanism=SCRAM-SHA-256&retrywrites=false&maxIdleTimeMS=120000");
                var database = client.GetDatabase("Argentina");
                var collection = database.GetCollection<BsonDocument>("Hotel");

                var jsonString = "{ \"key\": \"value\" }"; // Replace with your JSON string


                foreach (var hotel in hotels)
                {
                    string request = @"
                           <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns=""http://www.juniper.es/webservice/2007/"">
                              <soapenv:Header/>
                              <soapenv:Body>
                                <HotelContent>
                                  <HotelContentRQ Version=""1.1"" Language=""en"">
                                   <Login Email=""##Username##"" Password=""##Password##""/>
                                    <HotelContentList>
                                      <Hotel Code=""##HotelJPCode##""/>
                                    </HotelContentList>
                                  </HotelContentRQ>
                                </HotelContent>
                              </soapenv:Body>
                            </soapenv:Envelope>
                            ";
                    request = request.Replace("##HotelJPCode##", hotel.JPCode);
                    var HotelsResponse = await Common.GetResponse(request, "HotelContent", "POST");
                    JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };

                    XDocument xmlDoc = XDocument.Parse(HotelsResponse.Body.ToString());

                    // Find a specific element using LINQ to XML
                    XNamespace soap = "http://schemas.xmlsoap.org/soap/envelope/";
                    XNamespace juniper = "http://www.juniper.es/webservice/2007/";

                    // Assuming you want to retrieve hotellist element
                    XElement HotelListElement = xmlDoc.Root?
                        .Element(soap + "Body")?
                        .Element(juniper + "HotelContentResponse")?
                        .Element(juniper + "ContentRS")?
                        .Element(juniper + "Contents");

                    //string jsonString = ConvertXmlToJson(HotelListElement.ToString());

                    var bsonDocument = BsonDocument.Parse(jsonString);

                    collection.InsertOne(bsonDocument);

                    Console.WriteLine("Document inserted successfully!");

                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        static string ConvertXmlToJson(string xmlString)
        {
            // Load XML string into XDocument
            XDocument xmlDoc = XDocument.Parse(xmlString);

            // Convert XDocument to JSON
            JObject json = new JObject();
            TraverseXmlNode(json, xmlDoc.Root);

            // Convert JObject to JSON string
            return json.ToString();
        }

        static void TraverseXmlNode(JObject json, XElement element)
        {
            foreach (XElement childElement in element.Elements())
            {
                if (childElement.HasElements)
                {
                    // Recursively traverse child elements
                    JObject childJson = new JObject();
                    TraverseXmlNode(childJson, childElement);
                    json[childElement.Name.LocalName] = childJson;
                }
                else
                {
                    // Add simple element as property
                    json[childElement.Name.LocalName] = childElement.Value;
                }
            }
        }

        int counter = 0;
        int noProcessHotelCounter = 0;
        public async Task<bool> GetDataFromSupplier()
        {
            MetaResponse responseHotelList = null;
            Dictionary<int, DateTime> HotelCodesInDB = null;

            RedisProvider redisProvider = new RedisProvider();
            redisProvider.InitializeConnection(Environment.GetRedisConnectionString());

            var countriesForHotels = await redisProvider.GetDataAll("locations:countries");

            foreach (var country in countriesForHotels)
            {

                var countryCode = country.Name.ToString();
                var countryName = country.Value.ToString();

                responseHotelList = await CallSupplierForHotelList(1, 1000, countryCode);
                if (responseHotelList != null && responseHotelList.hotels != null && responseHotelList.hotels.Count > 0)
                {
                    HotelCodesInDB = await GetHotelCodeBysContryCode(countryCode);
                    await ProcessHotels(redisProvider, HotelCodesInDB, responseHotelList.hotels);
                    HotelCodesInDB = null;
                }
                if (responseHotelList != null && responseHotelList.hotels != null && responseHotelList.hotels.Count > 0)
                {
                    while (responseHotelList.to < responseHotelList.total)
                    {
                        responseHotelList = await CallSupplierForHotelList(responseHotelList.to + 1, responseHotelList.to + 1000, countryCode);
                        HotelCodesInDB = await GetHotelCodeBysContryCode(countryCode);
                        await ProcessHotels(redisProvider, HotelCodesInDB, responseHotelList.hotels);
                        HotelCodesInDB = null;
                    }
                }
            }
            redisProvider = null;
            redisProvider = null;
            countriesForHotels = null;
            responseHotelList = null;
            HotelCodesInDB = null;
            return true;
        }

        public async Task<MetaResponse> CallSupplierForHotelList(int fromNumber, int toNumber, string countryCode)
        {
            var hotelListResponse = await Common.GetResponse(String.Empty, "/hotels?fields=all&countryCode=" + countryCode + "&language=ENG&from=" + fromNumber + "&to=" + toNumber + "&useSecondaryLanguage=false", "GET");
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            var response = JsonConvert.DeserializeObject<MetaResponse>(hotelListResponse.Body, jsonSerializerSettings);
            return response;
        }
        public async Task<string> ProcessHotels(RedisProvider redisProvider, Dictionary<int, DateTime> HotelCodesInDB, List<Hotel> supplierHotels)
        {
            //Transform Hotel
            foreach (Hotel hotel in supplierHotels)
            {
                var watch = new System.Diagnostics.Stopwatch();
                watch.Start();
                counter++;
                
                if (HotelCodesInDB.ContainsKey(hotel.code))
                {
                    var dbLastDate = HotelCodesInDB[hotel.code];
                    if (Convert.ToDateTime(hotel.lastUpdate) == dbLastDate)
                    {
                        HotelCodesInDB.Remove(hotel.code);
                        watch.Stop();
                        Console.WriteLine($"* Hotel : {counter} : Execution Time: {watch.ElapsedMilliseconds} ms");
                        continue;
                    }
                }
                var hotelMeta = await TransformData(redisProvider,counter, hotel);
                //Save to database
                if (hotelMeta != null)
                    SaveToDatabase(hotelMeta);
                //Store it to redis
                try
                {
                    if (hotelMeta != null)
                        setToCache(redisProvider, hotelMeta);
                }
                catch(Exception ex)
                {
                    if (hotelMeta != null)
                        setToCache(redisProvider, hotelMeta);
                }
                watch.Stop();
                Console.WriteLine($"Hotel : {counter} : Execution Time: {watch.ElapsedMilliseconds} ms");
                watch = null;
            }
            return String.Empty;
        }
        public async Task<HotelMetaDTO> TransformData(RedisProvider redisProvider, int counter, Hotel supplierHotel)
        {
            try
            {
                var hotelCountryName = (string)(await redisProvider.GetData("locations:countries", supplierHotel.countryCode.ToString()));
                var hotelStateName = (string)(await redisProvider.GetData("locations:states:" + supplierHotel.countryCode, supplierHotel.stateCode));
                var hotelDestinationName = (string)(await redisProvider.GetData("destination:" + supplierHotel.destinationCode, "destinationName"));
                var hotelZoneName = (string)(await redisProvider.GetData("destination:" + supplierHotel.destinationCode, "destinationName"));
                var hotelAccommodationType = supplierHotel.accommodationTypeCode != null ? (string)(await redisProvider.GetData("types:accommodation", supplierHotel.accommodationTypeCode)) : null;

                List<string> boards = new List<string>();
                if (supplierHotel.facilities != null)
                {
                    foreach (Facility facility in supplierHotel.facilities)
                    {
                        var redisValue = await redisProvider.GetData("types:facilities", facility.facilityCode.ToString() + ":" + facility.facilityGroupCode.ToString(), true);
                        var facilityRedisValue = (Facility)Deserialize(redisValue);
                        facility.facilityName = facilityRedisValue.facilityName;
                        facility.facilityGroupName = facilityRedisValue.facilityGroupName;
                    }
                }
                if (supplierHotel.boardCodes != null)
                {
                    foreach (var boardCode in supplierHotel.boardCodes)
                    {
                        var boardName = await redisProvider.GetData("types:board", boardCode);
                        boards.Add(boardName.ToString());
                    }
                }
                var newHotel = new HotelMetaDTO()
                {
                    hotelCode = supplierHotel.code.ToString(),
                    hotelCommonCode = supplierHotel.code.ToString(),
                    hotelName = supplierHotel.name.content,
                    description = supplierHotel.description?.content,
                    countryCode = supplierHotel.countryCode.ToString(),
                    countryName = hotelCountryName,
                    stateCode = supplierHotel?.stateCode,
                    stateName = hotelStateName,
                    destincationCode = supplierHotel?.destinationCode.ToString(),
                    destinationName = hotelDestinationName,
                    zoneCode = supplierHotel.zoneCode.ToString(),
                    zoneName = hotelZoneName,
                    latitude = supplierHotel.coordinates?.latitude ?? 0,
                    longitude = supplierHotel.coordinates?.longitude ?? 0,
                    //hotelChain = supplierHotel.chain.description.content,//?
                    accommodationTypeCode = supplierHotel.accommodationTypeCode,
                    accommodationTypeName = hotelAccommodationType,
                    boards = boards,
                    address = supplierHotel.address?.content,
                    postalCode = supplierHotel.postalCode,
                    city = supplierHotel.city?.content,
                    email = supplierHotel.email,
                    phone1 = supplierHotel.phones?.FirstOrDefault()?.phoneNumber,
                    phone2 = supplierHotel.phones?.Skip(1).Take(1)?.FirstOrDefault()?.phoneNumber,
                    hotelFacilities = supplierHotel.facilities?.Where(x => x.facilityGroupCode == 70).Select(x => x.facilityName).ToList(),
                    websiteURL = supplierHotel.web,
                    starRating = supplierHotel.categoryCode?.Remove(1),
                    distanceFromCityCenter = supplierHotel.facilities?.Where(x => x.facilityGroupCode == 40 && x.facilityCode == 10).Count() == 0 ? null :
                    new DistanceFromCityCenter() { unit = "", unitValue = supplierHotel.facilities?.Where(x => x.facilityGroupCode == 40 && x.facilityCode == 10).FirstOrDefault()?.distance },
                    hotelImage = supplierHotel?.images?.Where(x => x.visualOrder == 0).Count() > 0 ? "http://photos.hotelbeds.com/giata/" + supplierHotel.images.Where(x => x.visualOrder == 0).FirstOrDefault()?.path : "",
                    lastUpdate = supplierHotel.lastUpdate,
                    rooms = supplierHotel.rooms
                };
                return newHotel;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<dynamic> SaveToDatabase(HotelMetaDTO hotel)
        {
            DBConnect? obj = new DBConnect();
            {
                try
                {
                    obj.Insert("delete from hotel where hotelCode = " + hotel.hotelCode);
                    obj.Insert("insert into hotel(hotelCode, countryCode, destinationCode, hotelData, lastUpdate) values('" + hotel.hotelCode + "', '" + hotel.countryCode + "', '" + hotel.destincationCode + "', '" + JsonConvert.SerializeObject(hotel).Replace("'", "''") + "', '" + hotel.lastUpdate + "')");
                }
                catch (Exception ex)
                {
                    obj = null;
                }
            }
            obj = null;
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
        private async Task<Dictionary<int, DateTime>> GetHotelCodeBysContryCode(string countryCode)
        {
            Dictionary<int,DateTime> hotelCodeList = new Dictionary<int, DateTime>();
            DBConnect? dBConnect = new DBConnect();
            MySqlDataReader dataReader = null;
            try
            {
                dataReader = await dBConnect.Select("select date_format(lastUpdate,'%Y-%m-%d') as 'lastUpdate', HotelCode from hotel where countryCode = '" + countryCode + "';");

                while (dataReader.Read())
                {
                    hotelCodeList.Add(int.Parse(dataReader["HotelCode"].ToString()), Convert.ToDateTime(dataReader["lastUpdate"]));
                }

                dataReader.Close();

                dBConnect.CloseConnection();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                dBConnect = null;
                dataReader = null;
            }
            return hotelCodeList;
        }
        public async void setToCache(RedisProvider redisProvider, HotelMetaDTO hotel)
        {
            await redisProvider.SetData("hotels:" + hotel.hotelCode, JsonConvert.SerializeObject(hotel));
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