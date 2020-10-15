using AirandWebAPI.Services.Contract;
using AirandWebAPI.Core;
using AirandWebAPI.Core.Domain;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using System;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using AirandWebAPI.Helpers;
using AirandWebAPI.Models.Auth;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirandWebAPI.Models.Direction;

namespace AirandWebAPI.Services.Concrete
{

    public class LocationService : ILocationService
    {
        private IUnitOfWork _unitOfWork;
        private readonly AppSettings _appSettings;

        public LocationService(IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings)
        {
            _unitOfWork = unitOfWork;
            _appSettings = appSettings.Value;
        }

        public async Task<bool> UpdateDriverLocation(Coordinates model, int userId)
        {
            var rider = _unitOfWork.Riders.Find(x => x.UserId.Equals(userId)).FirstOrDefault();
            if(rider != null){
                rider.Latitude = model.latitude;
                rider.Longitude = model.longitude;
                rider.LastModified = DateTime.Now;
                await _unitOfWork.Complete();
                return true;
            }
            return false;
        }
    }
}