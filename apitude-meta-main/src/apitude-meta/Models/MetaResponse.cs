namespace apitude_meta.Models
{
    public class MetaResponse
    {
        public int from { get; set; }
        public int to { get; set; }
        public int total { get; set; }
        public AuditData auditData { get; set; }
        public List<Accommodation> accommodations { get; set; }
        public List<RateClass> classifications { get; set; }
        public List<Board> boards { get; set; }
        public List<Promotion> promotions { get; set; }
        public List<Room> rooms{ get; set; }
        public List<Country> countries { get; set; }
        public List<Destination> destinations { get; set; }
        public List<Hotel> hotels { get; set; }
        public List<Facility> facilities { get; set; }
        public List<FacilityGroups> facilityGroups { get; set; }
        public List<Zones> zones { get; set; }
    }
    public class AuditData
    {
        public string processTime { get; set; }
        public string timestamp { get; set; }
        public string requestHost { get; set; }
        public string serverId { get; set; }
        public string environment { get; set; }
        public string release { get; set; }
        public string token { get; set; }
        public string @internal { get; set; }
    }
}
