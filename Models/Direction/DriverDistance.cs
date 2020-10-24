namespace AirandWebAPI.Models.Direction
{
    public class DriverDistance{
        public int driverId { get; set; }
        public Distance distance { get; set; }
        public Duration duration { get; set; }

        public DriverDistance(int driverId, Distance distance, Duration duration){
            this.driverId = driverId;
            this.distance = distance;
            this.duration = duration;
        }

        public DriverDistance(){}
    }
}