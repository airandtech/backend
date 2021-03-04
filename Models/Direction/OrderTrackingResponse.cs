using AirandWebAPI.Core.Domain;

namespace AirandWebAPI.Models.Direction
{
    public class OrderTrackingResponse{
        public OrderTrackingResponse(){}
        public OrderTrackingResponse(Order order, Rider rider){
            this.rider = new Coords(rider.Latitude, rider.Longitude);
            this.destination = new Coords(order.Delivery.Lat, order.Delivery.Lng);
            this.deliveryAddress = order.Delivery.Address;
            this.pickUpAddress = order.PickUp.Address;
            this.deliveryStatus = order.Status;
            this.deliveryFee = order.Cost;
            this.riderName = $"{rider.User.FirstName} {rider.User.LastName}";
            this.riderPhone = rider.User.Phone;
        }
        public Coords rider { get; set; }
        public Coords destination { get; set; }
        public string pickUpAddress { get; set; }
        public string deliveryAddress { get; set; }
        public string deliveryStatus { get; set; }
        public string riderName { get; set; }
        public string riderPhone { get; set; }
        public decimal deliveryFee { get; set; }
    }

    public class Coords{
        public string lat { get; set; }
        public string lng { get; set; }

        public Coords(string latitude, string longitude){
            this.lat = latitude;
            this.lng = longitude;
        }

        public Coords(){}
    }
}