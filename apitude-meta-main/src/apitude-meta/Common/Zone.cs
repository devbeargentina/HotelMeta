using System;
using System.Xml.Serialization;
using System.Collections.Generic;

// Define classes to represent the XML structure

[XmlRoot(ElementName = "Zone")]
public class Zone
{
    [XmlAttribute(AttributeName = "JPDCode")]
    public string JPDCode { get; set; }

    [XmlAttribute(AttributeName = "ParentJPDCode")]
    public string ParentJPDCode { get; set; }

    [XmlAttribute(AttributeName = "AreaType")]
    public string AreaType { get; set; }

    [XmlAttribute(AttributeName = "Searchable")]
    public string Searchable { get; set; }

    [XmlAttribute(AttributeName = "Code")]
    public string Code { get; set; }

    [XmlAttribute(AttributeName = "ParentCode")]
    public string ParentCode { get; set; }

    [XmlElement(ElementName = "Name")]
    public string Name { get; set; }
}

[XmlRoot(ElementName = "ZoneList")]
public class ZoneList
{
    [XmlElement(ElementName = "Zone")]
    public List<Zone> Zones { get; set; }
}

[XmlRoot(ElementName = "ZoneListRS")]
public class ZoneListRS
{
    [XmlElement(ElementName = "ZoneList")]
    public ZoneList ZoneList { get; set; }

    [XmlAttribute(AttributeName = "Url")]
    public string Url { get; set; }

    [XmlAttribute(AttributeName = "TimeStamp")]
    public string TimeStamp { get; set; }

    [XmlAttribute(AttributeName = "IntCode")]
    public string IntCode { get; set; }
}

[XmlRoot(ElementName = "ZoneListResponse", Namespace = "http://www.juniper.es/webservice/2007/")]
public class ZoneListResponse
{
    [XmlElement(ElementName = "ZoneListRS", Namespace = "http://www.juniper.es/webservice/2007/")]
    public ZoneListRS ZoneListRS { get; set; }
}

