using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AirandWebAPI.Models.Direction;
using AirandWebAPI.Services.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace AirandWebAPI.Utils
{
    public static class DistanceCalculator
    {
        public static async Task Process(
            Dictionary<string, string> destinations,
            string sourceLatitude,
            string sourceLongitude)
        {

            await getDistances(destinations, sourceLatitude, sourceLongitude);
        }

        public static async Task<DistanceMatrixResponse> getDistances(
            Dictionary<string, string> destinations,
            string sourceLatitude,
            string sourceLongitude
        )
        {
            DistanceMatrixResponse distanceMatrix = new DistanceMatrixResponse();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://maps.googleapis.com/maps/api/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // New code:
                HttpResponseMessage response = await client.GetAsync("api/products/1");
                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    distanceMatrix = JsonConvert.DeserializeObject<DistanceMatrixResponse>(responseString);
                }
            }
            return distanceMatrix;
        }


    }
}