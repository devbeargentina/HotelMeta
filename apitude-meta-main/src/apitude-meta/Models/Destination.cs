namespace apitude_meta.Models
{
    public class Destination
    {
        public string code { get; set; }
        public Name name { get; set; }
        public string countryCode { get; set; }
        public string isoCode { get; set; }
        public List<Zone> zones { get; set; }
        public List<GroupZone> groupZones { get; set; }
    }

    public class GroupZone
    {
        public string groupZoneCode { get; set; }
        public Name name { get; set; }
        public List<int> zones { get; set; }
    }

    public class Name
    {
        public string content { get; set; }
    }


    public class Zone
    {
        public int zoneCode { get; set; }
        public string name { get; set; }
        public Description description { get; set; }
    }
    public class NewDestination
    {
        public string DestinationCode { get; set; }
        public string DestinationName { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public int ZoneCode { get; set; }
        public string ZoneName { get; set; }
        public string DisplayName { get; set; }

    }

}
