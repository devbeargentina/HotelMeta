namespace apitude_meta.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class AccommodationType
    {
        public string code { get; set; }
        public TypeMultiDescription typeMultiDescription { get; set; }
        public string typeDescription { get; set; }
    }

    public class Address
    {
        public string content { get; set; }
        public string street { get; set; }
    }

    public class Category
    {
        public string code { get; set; }
        public Description description { get; set; }
    }

    public class CategoryGroup
    {
        public string code { get; set; }
        public Description description { get; set; }
    }

    public class Chain
    {
        public string code { get; set; }
        public Description description { get; set; }
    }

    public class Characteristic
    {
        public string code { get; set; }
        public Description description { get; set; }
    }

    public class City
    {
        public string content { get; set; }
    }

    public class Coordinates
    {
        public double longitude { get; set; }
        public double latitude { get; set; }
    }

    public class Facility
    {
        public int code { get; set; }
        public int facilityCode { get; set; }
        public string facilityName { get; set; }
        public int facilityGroupCode { get; set; }
        public string facilityGroupName { get; set; }
        public Description description { get; set; }
        public int facilityTypologyCode { get; set; }
        public int order { get; set; }
        public int number { get; set; }
        public bool voucher { get; set; }
        public bool? indYesOrNo { get; set; }
        public bool? indLogic { get; set; }
        public bool? indFee { get; set; }
        public string timeFrom { get; set; }
        public string timeTo { get; set; }
        public string distance { get; set; }
    }
    public class FacilityGroups
    {
        public int code { get; set; }
        public Description description { get; set; }
    }
    public class Hotel
    {
        public int code { get; set; }
        public Name name { get; set; }
        public string countryCode { get; set; }
        public string stateCode { get; set; }
        public string destinationCode { get; set; }
        public string zoneCode { get; set; }
        public string categoryCode { get; set; }
        public string categoryGroupCode { get; set; }
        public string chainCode { get; set; }
        public string accommodationTypeCode { get; set; }
        public List<string> boardCodes { get; set; }
        public List<int> segmentCodes { get; set; }
        public Description description { get; set; }
        public Country country { get; set; }
        public State state { get; set; }
        public Destination destination { get; set; }
        public Zone zone { get; set; }
        public Coordinates coordinates { get; set; }
        public Category category { get; set; }
        public CategoryGroup categoryGroup { get; set; }
        public Chain chain { get; set; }
        public AccommodationType accommodationType { get; set; }
        public List<Board> boards { get; set; }
        public List<Segment> segments { get; set; }
        public Address address { get; set; }
        public string postalCode { get; set; }
        public City city { get; set; }
        public string email { get; set; }
        public List<Phone> phones { get; set; }
        public List<Room> rooms { get; set; }
        public List<Facility> facilities { get; set; }
        public List<Terminal> terminals { get; set; }
        public List<Issue> issues { get; set; }
        public List<Image> images { get; set; }
        public List<Wildcard> wildcards { get; set; }
        public string web { get; set; }
        public string lastUpdate { get; set; }
        public long ranking { get; set; }
        public string S2C { get; set; }
    }

    public class Image
    {
        public Type type { get; set; }
        public string path { get; set; }
        public int order { get; set; }
        public int visualOrder { get; set; }
        public string roomCode { get; set; }
        public string roomType { get; set; }
        public string characteristicCode { get; set; }
    }

    public class Issue
    {
        public string issueCode { get; set; }
        public string issueType { get; set; }
        public Description description { get; set; }
        public string dateFrom { get; set; }
        public string dateTo { get; set; }
        public int order { get; set; }
        public bool alternative { get; set; }
    }

    public class Phone
    {
        public string phoneNumber { get; set; }
        public string phoneType { get; set; }
    }

    public class RoomStay
    {
        public string stayType { get; set; }
        public string order { get; set; }
        public string description { get; set; }
        public List<RoomStayFacility> roomStayFacilities { get; set; }
    }

    public class RoomStayFacility
    {
        public int facilityCode { get; set; }
        public string facilityName { get; set; }
        public int facilityGroupCode { get; set; }
        public Description description { get; set; }
        public int number { get; set; }
    }

    public class Segment
    {
        public int code { get; set; }
        public Description description { get; set; }
    }

    public class Type
    {
        public string code { get; set; }
        public Description description { get; set; }
    }
    public class Terminal
    {
        public string terminalCode { get; set; }
        public int distance { get; set; }
    }
    public class HotelRoomDescription
    {
        public string content { get; set; }
    }

    public class Wildcard
    {
        public string roomType { get; set; }
        public string roomCode { get; set; }
        public string characteristicCode { get; set; }
        public HotelRoomDescription hotelRoomDescription { get; set; }
    }

    public class DistanceFromCityCenter
    {
        public string unit { get; set; }
        public string unitValue { get; set; }

    }
    public class HotelMetaDTO
    {
        public string hotelCommonCode { get; set; }
        public string hotelCode { get; set; }
        public string hotelName { get; set; }
        public string description { get; set; }
        public string countryCode { get; set; }
        public string countryName { get; set; }
        public string stateCode{ get; set; }
        public string stateName { get; set; }
        public string destincationCode { get; set; }
        public string destinationName { get; set; }
        public string zoneCode { get; set; }
        public string zoneName { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string hotelChain { get; set; }
        public string accommodationTypeCode { get; set; }
        public string accommodationTypeName { get; set; }
        public List<string> boards { get; set; }
        public string address { get; set; }
        public string postalCode { get; set; }
        public string city { get; set; }
        public string email { get; set; }
        public string phone1 { get; set; }
        public string phone2 { get; set; }
        public List<string> hotelFacilities { get; set; }
        public string websiteURL { get; set; }
        public string starRating { get; set; }
        public List<string> roomFacilities { get; set; }
        //Rooms
        public DistanceFromCityCenter distanceFromCityCenter { get; set; }
        public string hotelImage { get; set; }
        public List<string> hotelImages { get; set; }
        public List<Room> rooms { get; set; }
        public string lastUpdate { get; set; }
    }
}