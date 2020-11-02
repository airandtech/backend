using System;
using System.Net;
using AirandWebAPI.Helpers;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using AirandWebAPI.Models;
using AirandWebAPI.Models.Dispatch;
using AirandWebAPI.Models.Direction;

namespace AirandWebAPI.Services
{

    public class NotificationService : INotification
    {
        private readonly AppSettings _appSettings;

        private readonly string apiKey = "AAAAeVEOA94:APA91bGXoaBNHuLvmKCFZ5TvL4M9kFVdSvOiR4hbYCo2gR4IRA1EpnLb-PSTCXFgQ6E_0QKFd8J3lP_2slQVoxdwM688L1ZEVXvpCZfCAGEUuoHuqoGLw8CVa3dDjH-gCYUfU6gfO_wN";

     

        private static RideOrderRequest requestData;
        private static DriverDistance driverDistance;

        public NotificationService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public void setRequestData(RideOrderRequest model){
            requestData = new RideOrderRequest();
            requestData = model;
        }
        public void setDriverDistance(DriverDistance model){
            driverDistance = new DriverDistance();
            driverDistance = model;
        }

        public async Task<bool> SendAsync(string title, string message, string mobile_token)
        {
            HttpClient client = new HttpClient();
            if(requestData == null) return false;
            
            client.BaseAddress = new Uri("https://fcm.googleapis.com/fcm/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=" + apiKey);

            Notification notification = new Notification(title, message, "");
            Data data = new Data(requestData, driverDistance);
            PushNotification pushObj = new PushNotification(notification, new List<string>(){mobile_token}, data);
           

            HttpResponseMessage response = await client.PostAsJsonAsync("send", pushObj);
            //response.EnsureSuccessStatusCode();

            if(response.StatusCode == HttpStatusCode.OK) return true;

            return false;
        }
    }
}