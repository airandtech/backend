using System.Collections.Generic;
using AirandWebAPI.Models.Direction;
using AirandWebAPI.Models.Dispatch;

namespace AirandWebAPI.Models
{

    public class Notification
    {
        public Notification(string title, string body, string image)
        {
            this.title = title;
            this.body = body;
            this.image = image;
        }

        public string title { get; set; }
        public string body { get; set; }
        public string image { get; set; }
    }

    public class Data
    {
        public Data(RideOrderRequest requestData, DriverDistance driverDistance)
        {
            this.requestorEmail = requestData.PickUp.Email;
            this.requestorName = requestData.PickUp.Name;
            this.address = requestData.PickUp.Address;
            this.distance = driverDistance.distance.text;
            this.time = driverDistance.duration.text;
        }

        public string requestorEmail { get; set; }
        public string requestorName { get; set; }
        public string address { get; set; }
        public string time { get; set; }
        public string distance { get; set; }
    }

    public class PushNotification
    {
        public PushNotification(Notification notification, List<string> registration_ids, Data data)
        {
            this.notification = notification;
            this.registration_ids = registration_ids;
            this.data = data;
        }

        public Notification notification { get; set; }
        public IList<string> registration_ids { get; set; }
        public Data data { get; set; }
    }

}