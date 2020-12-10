using System;
using System.Net;
using AirandWebAPI.Helpers;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using AirandWebAPI.Models;
using System.Text;
using Newtonsoft.Json;

namespace AirandWebAPI.Services
{

    public class SmsService : ISmsService
    {
        private readonly AppSettings _appSettings;

        //private readonly string apiKey = "MmRkYmRjMzA5MmMwZmFmMjNjMGQwYThhY2Q3OTcxOWE6MGM2OWJhNGFiYzk5MjBlMzliNTk4ZmQ2OTEwYjYwYjg=";
        private readonly string apiKey = "YjJlNGQ2YzA3ZjRlMjUyYjUxZjY1ZjlhMzA4NTMwZGI6NzQ5MzBmY2MxMDJjOGQ2NmEwY2FiYzVmYWFjZGUzZGY=";
        

        public SmsService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public async Task<bool> SendAsync(SmsBody model)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://jusibe.com/smsapi/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", apiKey);

            SmsBody jsonObject = new SmsBody(model.from, model.to, model.message);

            string json = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);

            var body = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsJsonAsync("send_sms", jsonObject);
            //response.EnsureSuccessStatusCode();

            if(response.StatusCode == HttpStatusCode.OK) return true;
            string responseBody = await response.Content.ReadAsStringAsync();
            return false;
        }
    }
}