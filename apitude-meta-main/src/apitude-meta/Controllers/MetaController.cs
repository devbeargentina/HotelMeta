using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using apitude_meta.Common;
using DataProviders;
using Newtonsoft.Json;

namespace apitude_meta.Controllers
{
    [Route("api/[controller]")]
    public class MetaController : ControllerBase
    {
        //[HttpPost]
        //public async Task<dynamic> Post([FromBody] string value)
        //{
        //    /*
        //    //****Types****
        //    //Accommodations
        //    TypesAccommodations objAccommodations = new TypesAccommodations();
        //    var accommodations = await objAccommodations.GetDataFromSupplier();
        //    await objAccommodations.SaveToDatabase(accommodations);
        //    objAccommodations.setToCache(accommodations);

        //    //Rate Class (Classification)
        //    TypesRateClass objRateClass = new TypesRateClass();
        //    var rateclass= await objRateClass.GetDataFromSupplier();
        //    await objRateClass.SaveToDatabase(rateclass);
        //    objRateClass.setToCache(rateclass);

        //    //Boards
        //    TypesBoards objTypesBoards = new TypesBoards();
        //    var boards = await objTypesBoards.GetDataFromSupplier();
        //    await objTypesBoards.SaveToDatabase(boards);
        //    objTypesBoards.setToCache(boards);

        //    //Promotions
        //    TypesPromotions objTypesPromotions = new TypesPromotions();
        //    var promotions = await objTypesPromotions.GetDataFromSupplier();
        //    await objTypesPromotions.SaveToDatabase(promotions);
        //    objTypesPromotions.setToCache(promotions);

        //    //Rooms
        //    TypesRooms objTypesRooms = new TypesRooms();
        //    var rooms = await objTypesRooms.GetDataFromSupplier();
        //    await objTypesRooms.SaveToDatabase(rooms);
        //    objTypesRooms.setToCache(rooms);

        //    //****Locations****
        //    //Countries
        //    LocationsCountries objCountries= new LocationsCountries();
        //    var countries = await objCountries.GetDataFromSupplier();
        //    await objCountries.SaveToDatabase(countries);
        //    objCountries.setToCache(countries);

        //    //Destination
        //    LocationsDestination objDestination = new LocationsDestination();
        //    var destinations = await objDestination.GetDataFromSupplier();
        //    await objDestination.SaveToDatabase(destinations);
        //    objDestination.setToCache(destinations);

        //    TypesFacilities objFacilities = new TypesFacilities();
        //    var facilities = await objFacilities.GetDataFromSupplierFacilities();
        //    await objFacilities.SaveToDatabase(facilities);
        //    objFacilities.setToCache(facilities);

        //    */

        //    //Zones
        //    LocationsZones objZones = new LocationsZones();
        //    var zones = await objZones.GetDataFromSupplier();
        //    //await objCountries.SaveToDatabase(countries);
        //    //objCountries.setToCache(countries);

        //    /*
        //    HotelsMetas objHotels = new HotelsMetas();
        //    await objHotels.GetDataFromSupplier();

        //    var c = "";
        //    //List<Country> countries
        //    //TODO: Get categories
        //    //TODO: Get facilityGroup
        //    //TODO: Get Facility

        //    */
        //    return "ok";
        //}

        [HttpPost("zone")]
        public async Task<dynamic> Post([FromBody] string value)
        {
           

            //Zones
            LocationsZones objZones = new LocationsZones();
            var zones = await objZones.GetDataFromSupplier();
            //await objCountries.SaveToDatabase(countries);
            //objCountries.setToCache(countries);

            /*
            HotelsMetas objHotels = new HotelsMetas();
            await objHotels.GetDataFromSupplier();
            
            var c = "";
            //List<Country> countries
            //TODO: Get categories
            //TODO: Get facilityGroup
            //TODO: Get Facility

            */
            return "ok";
        }

        [HttpPost("city")]
        public async Task<dynamic> cityPost([FromBody] string value)
        {


            //Cities
            LocationsCities objCities = new LocationsCities();
            var cities = await objCities.GetDataFromSupplier();
            //await objCountries.SaveToDatabase(countries);
            //objCountries.setToCache(countries);

            /*
            HotelsMetas objHotels = new HotelsMetas();
            await objHotels.GetDataFromSupplier();
            
            var c = "";
            //List<Country> countries
            //TODO: Get categories
            //TODO: Get facilityGroup
            //TODO: Get Facility

            */
            return "ok";
        }

        [HttpPost("hotelportfolio")]
        public async Task<dynamic> hotelPortfolioPost([FromBody] string value)
        {


           
            HotelsJuniper objHotelPortfolio = new HotelsJuniper();
            var hotels = await objHotelPortfolio.GetDataFromSupplierJuniper();
            //await objCountries.SaveToDatabase(countries);
            //objCountries.setToCache(countries);

            /*
            HotelsMetas objHotels = new HotelsMetas();
            await objHotels.GetDataFromSupplier();
            
            var c = "";
            //List<Country> countries
            //TODO: Get categories
            //TODO: Get facilityGroup
            //TODO: Get Facility

            */
            return "ok";
        }
        

    }

}