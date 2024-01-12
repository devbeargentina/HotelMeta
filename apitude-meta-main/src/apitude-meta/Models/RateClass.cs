namespace apitude_meta.Models
{
    public class RateClass
    {
        public string code { get; set; }
        public Description description { get; set; }
    }
    public class Description
    {
        public string languageCode { get; set; } //Boards
        public string content { get; set; } //Rate Class, Boards
    }

    public class NewRateClass
    {
        public string code { get; set; }
        public string data { get; set; }
    }

}
