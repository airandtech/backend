using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirandWebAPI.Services.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AirandWebAPI.Utils
{
    public static class PriceCalculator
    {
        public static decimal Process(string source, string destination)
        {

            int sourceWeight = getWeight(source);
            int destinationWeight = getWeight(destination);

            int scaledDistance = getScaledDistance(sourceWeight, destinationWeight);
            decimal cost = getCost(scaledDistance);

            return cost;
        }


        private static int getWeight(string code)
        {
            int weight;
            switch (code)
            {
                case "ISL001":
                    weight = 5;
                    break;
                case "ISL002":
                    weight = 4;
                    break;
                case "ISL003":
                    weight = 3;
                    break;
                case "ISL004":
                    weight = 2;
                    break;
                case "ISL005":
                    weight = 1;
                    break;
                case "MLD001":
                    weight = 6;
                    break;
                case "MLD002":
                    weight = 7;
                    break;
                case "MLD003":
                    weight = 8;
                    break;
                case "MLD004":
                    weight = 9;
                    break;
                case "MLD005":
                    weight = 10;
                    break;
                default:
                    weight = 10;
                    break;
            }
            return weight;
        }

        private static int getScaledDistance(int source, int destination)
        {
            return Math.Abs(source - destination);
        }

        private static decimal getCost(int scaledDistance)
        {

            if (scaledDistance == 0 || scaledDistance == 1)
            {
                return 1000;
            }
            if (scaledDistance == 2 || scaledDistance == 3)
            {
                return 1500;
            }
            if (scaledDistance == 4 || scaledDistance == 5)
            {
                return 2000;
            }
            if (scaledDistance == 6 || scaledDistance == 7)
            {
                return 2500;
            }
            if (scaledDistance == 8 || scaledDistance == 9)
            {
                return 3000;
            }
            return 4000;
        }
    }
}