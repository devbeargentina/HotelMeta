namespace apitude_meta.Models
{
    public class Room
    {
        public string code { get; set; }
        public string type { get; set; }
        public string characteristic { get; set; }
        public int minPax { get; set; }
        public int maxPax { get; set; }
        public int maxAdults { get; set; }
        public int maxChildren { get; set; }
        public int minAdults { get; set; }
        public string description { get; set; }
        public TypeDescription typeDescription { get; set; }
        public CharacteristicDescription characteristicDescription { get; set; }
        public string roomCode { get; set; }
        public bool isParentRoom { get; set; }
        //public Type type { get; set; }
        //public Characteristic characteristic { get; set; }
        public List<RoomStay> roomStays { get; set; }
    }

    public class TypeDescription
    {
        public string languageCode { get; set; }
        public string content { get; set; }
    }

    public class CharacteristicDescription
    {
        public string languageCode { get; set; }
        public string content { get; set; }
    }

    public class NewRoom
    {
        public string code { get; set; }
        public string type { get; set; }
        public string characteristic { get; set; }
        public int minPax { get; set; }
        public int maxPax { get; set; }
        public int maxAdults { get; set; }
        public int maxChildren { get; set; }
        public int minAdults { get; set; }
        public string description { get; set; }
        public string typeDescription { get; set; }
        public string characteristicDescription { get; set; }
    }
}
