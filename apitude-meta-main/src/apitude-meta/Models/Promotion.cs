namespace apitude_meta.Models
{
    public class Promotion
    {
        public string code { get; set; }
        public Description description { get; set; }
        public Description name{ get; set; }
    }

    public class NewPromotions
    {
        public string code { get; set; }
        public string data { get; set; }
    }
}
