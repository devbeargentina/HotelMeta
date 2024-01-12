using System.Xml.Serialization;
using System.Collections.Generic;

[XmlRoot(ElementName = "City")]
public class City
{
    [XmlAttribute(AttributeName = "Id")]
    public string Id { get; set; }

    [XmlAttribute(AttributeName = "JPDCode")]
    public string JPDCode { get; set; }

    [XmlElement(ElementName = "Name")]
    public string Name { get; set; }

    [XmlElement(ElementName = "Country")]
    public Country Country { get; set; }

    [XmlElement(ElementName = "Region")]
    public Region Region { get; set; }
}

[XmlRoot(ElementName = "Country")]
public class Country
{
    [XmlAttribute(AttributeName = "Id")]
    public string Id { get; set; }

    [XmlAttribute(AttributeName = "JPDCode")]
    public string JPDCode { get; set; }

    [XmlElement(ElementName = "Name")]
    public string Name { get; set; }
}

[XmlRoot(ElementName = "Region")]
public class Region
{
    [XmlAttribute(AttributeName = "Id")]
    public string Id { get; set; }

    [XmlAttribute(AttributeName = "JPDCode")]
    public string JPDCode { get; set; }

    [XmlElement(ElementName = "Name")]
    public string Name { get; set; }
}

[XmlRoot(ElementName = "CityList")]
public class CityList
{
    [XmlElement(ElementName = "City")]
    public List<City> Cities { get; set; }
}

[XmlRoot(ElementName = "CityListRS")]
public class CityListRS
{
    [XmlElement(ElementName = "CityList")]
    public CityList CityList { get; set; }

    [XmlAttribute(AttributeName = "Url")]
    public string Url { get; set; }

    [XmlAttribute(AttributeName = "TimeStamp")]
    public string TimeStamp { get; set; }

    [XmlAttribute(AttributeName = "IntCode")]
    public string IntCode { get; set; }
}

[XmlRoot(ElementName = "CityListResponse", Namespace = "http://www.juniper.es/webservice/2007/")]
public class CityListResponse
{
    [XmlElement(ElementName = "CityListRS")]
    public CityListRS CityListRS { get; set; }
}
