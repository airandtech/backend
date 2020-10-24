namespace AirandWebAPI.Models.Direction
{
    public class DriverCoordinates{
        public int driverId { get; set; }
        public Coordinates coordinates { get; set; }

        public DriverCoordinates(int driverId, Coordinates coordinates){
            this.driverId = driverId;
            this.coordinates = coordinates;
        }

        public DriverCoordinates(){}
    }
}