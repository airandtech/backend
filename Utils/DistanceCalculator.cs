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
        private static readonly string key = "AIzaSyCnwbX1EzOAP_vNJ5pF2GFk8GycpY8WTBA";
        public static async Task<DistanceMatrixResponse> Process(
            List<DriverCoordinates> destinations,
            string sourceLatitude,
            string sourceLongitude)
        {
            var coordinatesHeaderParams = toHeaderParameters(destinations, sourceLatitude, sourceLongitude);
            DistanceMatrixResponse distanceMatrix = await getDistances(coordinatesHeaderParams);

            return distanceMatrix;
        }

         public static async Task<DirectionResponse> Process(
            Coordinates source,
            Coordinates destination)
        {
            string coordinatesHeaderParams = $"origin={source.latitude},{source.longitude}&destination={destination.latitude},{destination.longitude}";
            DirectionResponse distance = await getDistance(coordinatesHeaderParams);

            return distance;
        }

        private static string toHeaderParameters(List<DriverCoordinates> destinations, string sourceLatitude, string sourceLongitude)
        {
            string headers = "origins=";
            foreach (var entry in destinations)
            {
                
                headers += $"{entry.coordinates.latitude},{entry.coordinates.longitude}|";
            }

            headers = headers.Remove(headers.Length -1, 1);
            headers = $"{headers}&destinations={sourceLatitude},{sourceLongitude}";
            return headers;

        }

        private static async Task<DistanceMatrixResponse> getDistances( string coordinatesParams)
        {
            DistanceMatrixResponse distanceMatrix = new DistanceMatrixResponse();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://maps.googleapis.com/maps/api/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string headerParams = $"distancematrix/json?{coordinatesParams}&key={key}";
                // New code:
                HttpResponseMessage response = await client.GetAsync(headerParams);
                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    distanceMatrix = JsonConvert.DeserializeObject<DistanceMatrixResponse>(responseString);
                }
            }
            return distanceMatrix;
        }

        private static async Task<DirectionResponse> getDistance( string coordinatesParams)
        {
            DirectionResponse distance = new DirectionResponse();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://maps.googleapis.com/maps/api/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string headerParams = $"directions/json?{coordinatesParams}&key={key}";
                // New code:
                HttpResponseMessage response = await client.GetAsync(headerParams);
                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    distance = JsonConvert.DeserializeObject<DirectionResponse>(responseString);
                }
            }
            return distance;
        }




    }
}