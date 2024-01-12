namespace apitude_meta.Models
{
    public class Country
    {
        public string code { get; set; }
        public string isoCode { get; set; }
        public Description description { get; set; }
        public List<State> states { get; set; }
    }

    public class State
    {
        public string code { get; set; }
        public string name { get; set; }
    }


    public class NewCountry
    {
        public string code { get; set; }
        public string name { get; set; }
        public List<State> states { get; set; }

    }
    public class Zones
    {
        public string JPDCode { get; set; }
        public string ParentJPDCode { get; set; }
        public string AreaType { get; set; }
        public string Searchable { get; set; }
        public string Code { get; set; }
        public string ParentCode { get; set; }
        public string Name { get; set; }
    }
  public class  Cities
    {
        public string CityJPDCode { get; set; }
        public string CityName { get; set; }
        public string CountryId { get; set; }
        public string CountryJPDCode { get; set; }
        public string CountyName { get; set; }
        public string RegionId { get; set; }
        public string RegionJPDCode { get; set; }
        public string RegionName { get; set; }
    }
}
