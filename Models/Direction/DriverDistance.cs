namespace AirandWebAPI.Models.Direction
{
    public class DriverDistance{
        public int driverId { get; set; }
        public Distance distance { get; set; }

        public DriverDistance(int driverId, Distance distance){
            this.driverId = driverId;
            this.distance = distance;
        }

        public DriverDistance(){}
    }
}