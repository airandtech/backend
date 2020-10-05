namespace AirandWebAPI.Models.Direction
{
    public class Coordinates{
        public string latitude { get; set; }
        public string longitude { get; set; }

        public Coordinates(string latitude, string longitude){
            this.latitude = latitude;
            this.longitude = longitude;
        }

        public Coordinates(){}
    }
}