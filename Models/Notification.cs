using System.Collections.Generic;

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

    public class PushNotification
    {
        public PushNotification(Notification notification, List<string> registration_ids)
        {
            this.notification = notification;
            this.registration_ids = registration_ids;
        }

        public Notification notification { get; set; }
        public IList<string> registration_ids { get; set; }
    }

}