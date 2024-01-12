using System.Xml.Serialization;

[XmlRoot(ElementName = "HotelPortfolioResponse", Namespace = "http://www.juniper.es/webservice/2007/")]
public class HotelPortfolioResponse
{
    [XmlElement(ElementName = "HotelPortfolioRS")]
    public HotelPortfolioRS HotelPortfolioRS { get; set; }
}

public class HotelPortfolioRS
{
    [XmlAttribute(AttributeName = "Url")]
    public string Url { get; set; }

    [XmlAttribute(AttributeName = "TimeStamp")]
    public string TimeStamp { get; set; }

    [XmlAttribute(AttributeName = "IntCode")]
    public string IntCode { get; set; }

    [XmlElement(ElementName = "HotelPortfolio")]
    public HotelPortfolio HotelPortfolio { get; set; }
}

public class HotelPortfolio
{
    [XmlAttribute(AttributeName = "Page")]
    public string Page { get; set; }

    [XmlAttribute(AttributeName = "TotalPages")]
    public string TotalPages { get; set; }

    [XmlAttribute(AttributeName = "TotalRecords")]
    public string TotalRecords { get; set; }

    [XmlAttribute(AttributeName = "NextToken")]
    public string NextToken { get; set; }

    [XmlElement(ElementName = "Hotel")]
    public List<HotelInfoPortfolio> Hotel { get; set; }
}

[XmlType(TypeName = "JP_HotelInfoPortfolio")]
public class HotelInfoPortfolio
{
    [XmlAttribute(AttributeName = "JPCode")]
    public string JPCode { get; set; }

    [XmlAttribute(AttributeName = "HasSynonyms")]
    public string HasSynonyms { get; set; }

    [XmlElement(ElementName = "Name")]
    public string Name { get; set; }

    [XmlElement(ElementName = "Zone")]
    public Zone Zone { get; set; }

    [XmlElement(ElementName = "Address")]
    public string Address { get; set; }

    [XmlElement(ElementName = "ZipCode")]
    public string ZipCode { get; set; }

    [XmlElement(ElementName = "Latitude")]
    public string Latitude { get; set; }

    [XmlElement(ElementName = "Longitude")]
    public string Longitude { get; set; }

    [XmlElement(ElementName = "HotelCategory")]
    public HotelCategory HotelCategory { get; set; }

    [XmlElement(ElementName = "City")]
    public City City { get; set; }
}

//public class Zone
//{
//    [XmlAttribute(AttributeName = "JPDCode")]
//    public string JPDCode { get; set; }

//    [XmlAttribute(AttributeName = "Code")]
//    public string Code { get; set; }

//    [XmlElement(ElementName = "Name")]
//    public string Name { get; set; }
//}

public class HotelCategory
{
    [XmlAttribute(AttributeName = "Type")]
    public string Type { get; set; }

    [XmlAttribute(AttributeName = "Code")]
    public string Code { get; set; }

    [XmlText]
    public string Value { get; set; }
}

//public class City
//{
//    [XmlAttribute(AttributeName = "Id")]
//    public string Id { get; set; }

//    [XmlAttribute(AttributeName = "JPDCode")]
//    public string JPDCode { get; set; }

//    [XmlText]
//    public string Value { get; set; }
//}
