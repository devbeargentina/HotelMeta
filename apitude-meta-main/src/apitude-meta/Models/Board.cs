namespace apitude_meta.Models
{
    public class Board
    {
        public string code { get; set; }
        public Description description { get; set; }
        public string multiLingualCode { get; set; }
    }

    public class NewBoard
    {
        public string code { get; set; }
        public string data { get; set; }
    }
}
