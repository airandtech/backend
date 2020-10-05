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

namespace AirandWebAPI.Services.Concrete
{

    public class RegionService : IRegionService
    {
        private IUnitOfWork _unitOfWork;
        private readonly AppSettings _appSettings;

        public RegionService(IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings)
        {
            _unitOfWork = unitOfWork;
            _appSettings = appSettings.Value;
        }

        public IEnumerable<Region> GetAll()
        {
           return _unitOfWork.Regions.GetAll();
        }
    }
}