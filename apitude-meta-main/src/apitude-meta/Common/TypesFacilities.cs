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
    public class TypesFacilities
    {
        public async Task<List<FacilityGroups>> GetDataFromSupplierFacilityGroups()
        {
            MetaResponse response = await CallSupplierFacilityGroups();
            return response.facilityGroups;
        }

        public async Task<MetaResponse> CallSupplierFacilityGroups()
        {
            var RoomsResponse = await Common.GetResponse(String.Empty, "/types/facilitygroups?fields=all&language=ENG&from=1&to=100&useSecondaryLanguage=True", "GET");
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            var response = JsonConvert.DeserializeObject<MetaResponse>(RoomsResponse.Body, jsonSerializerSettings);
            return response;
        }

        public async Task<List<Facility>> GetDataFromSupplierFacilities()
        {
            List<Facility> facility = new List<Facility>();
            List<FacilityGroups> facilityGroups = await GetDataFromSupplierFacilityGroups();
            MetaResponse response = await CallSupplierFacilities();
            foreach (Facility obj in response.facilities)
            {
                obj.facilityCode = obj.code;
                obj.facilityName = obj.description?.content ?? "";
                obj.facilityGroupName = facilityGroups.Where(x => x.code==obj.facilityGroupCode).FirstOrDefault().description.content;
                facility.Add(obj);
            };
            return response.facilities;
        }

        public async Task<MetaResponse> CallSupplierFacilities()
        {
            var RoomsResponse = await Common.GetResponse(String.Empty, "/types/facilities?fields=all&language=ENG&from=1&to=1000&useSecondaryLanguage=True", "GET");
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            var response = JsonConvert.DeserializeObject<MetaResponse>(RoomsResponse.Body, jsonSerializerSettings);
            return response;
        }

        public async Task<dynamic> SaveToDatabase(List<Facility> facilities)
        {
            DBConnect? obj = new DBConnect();
            {
                obj.Insert("delete from facilities");
                foreach (Facility item in facilities)
                {
                    try
                    {
                        var existingData = await getExestingData(obj, item.facilityCode, item.facilityGroupCode);
                            
                        if (existingData == null)
                        {
                            obj.Insert("insert into facilities(facilityCode, facilityName, facilityGroupCode, facilityGroupName) values('" + item.facilityCode + "', '" + item.facilityName.Replace("'","''") + "', '" + item.facilityGroupCode + "', '" + item.facilityGroupName.Replace("'", "''") + "')");
                        }
                        //else
                        //{
                        //    obj.Insert("Update facilities set facilityName = '" + item.facilityName + "', " +
                        //        " facilityGroupName = '" + item.facilityGroupName + "' " +
                        //        "where facilityCode = '" + item.facilityCode + "' AND facilityGroupCode = '" + item.facilityGroupCode + "'");
                        //}
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return "success";
        }

        private async Task<Facility> getExestingData(DBConnect dBConnect, int facilityCode, int facilityGroupCode)
        {
            List<Facility> list = new List<Facility>();
            MySqlDataReader dataReader = await dBConnect.Select("select * from facilities where facilitycode = '" + facilityCode + "' AND facilityGroupCode = '" + facilityGroupCode + "'");
            try
            {
                while (dataReader.Read())
                {
                    list.Add(new Facility()
                    {
                        facilityCode = Convert.ToInt16(dataReader["facilityCode"].ToString()),
                        facilityName = dataReader["facilityName"].ToString(),
                        facilityGroupCode = Convert.ToInt16(dataReader["facilityGroupCode"].ToString()),
                        facilityGroupName = dataReader["facilityGroupName"].ToString(),
                    });
                }
            }
            catch (Exception ex)
            {
            }
            dataReader.Close();

            dBConnect.CloseConnection();

            return list.FirstOrDefault();
        }
        public async void setToCache(List<Facility> facilities)
        {
            RedisProvider redisProvider = new RedisProvider();
            redisProvider.InitializeConnection(Environment.GetRedisConnectionString());
            List<HashEntry> redisHash = new List<HashEntry>();

            foreach (var item in facilities)
            {
                redisHash.Add(new HashEntry(item.facilityCode + ":" + item.facilityGroupCode, item.Serialize()));
            }
            redisProvider.SetData("types:facilities", redisHash.ToArray());
        }
    }
}
