namespace apitude_meta.Models
{
    public class Accommodation
    {
        public string code { get; set; }
        public TypeMultiDescription typeMultiDescription { get; set; }
        public string typeDescription { get; set; }
    }

    public class TypeMultiDescription
    {
        public string languageCode { get; set; }
        public string content { get; set; }
    }

    public class NewAccommodation
    {
        public string code { get; set; }
        public string data { get; set; }
    }
}
