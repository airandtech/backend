using AirandWebAPI.Services.Contract;
using AirandWebAPI.Core;
using Microsoft.Extensions.Options;
using AirandWebAPI.Helpers;
using System.Threading.Tasks;
using AirandWebAPI.Core.Domain;
using System;

namespace AirandWebAPI.Services.Concrete
{

    public class MerchantService : IMerchantService
    {
        private IUnitOfWork _unitOfWork;
        private readonly AppSettings _appSettings;

        public MerchantService(IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings)
        {
            _unitOfWork = unitOfWork;
            _appSettings = appSettings.Value;
        }

        public async Task<Merchant> Create(Merchant merchant)
        {
            merchant.DateCreated = DateTime.UtcNow.AddHours(1);
            _unitOfWork.Merchants.Add(merchant);
            await _unitOfWork.Complete();
            return merchant;
        }
    }
}