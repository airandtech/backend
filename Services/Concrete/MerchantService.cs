using AirandWebAPI.Services.Contract;
using AirandWebAPI.Core;
using Microsoft.Extensions.Options;
using AirandWebAPI.Helpers;
using System.Threading.Tasks;
using AirandWebAPI.Core.Domain;
using System;
using System.IO;

namespace AirandWebAPI.Services.Concrete
{

    public class MerchantService : IMerchantService
    {
        private IUnitOfWork _unitOfWork;
        private readonly AppSettings _appSettings;
        private IMailer _mailer;

        public MerchantService(IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings, IMailer mailer)
        {
            _unitOfWork = unitOfWork;
            _appSettings = appSettings.Value;
            _mailer = mailer;
        }


        public async Task<Merchant> Create(Merchant merchant)
        {
            merchant.DateCreated = DateTime.UtcNow.AddHours(1);
            _unitOfWork.Merchants.Add(merchant);
            var saveTask =  _unitOfWork.Complete();
            var welcomeTask = welcomeMerchant(merchant);
            await Task.WhenAll(saveTask, welcomeTask);

            return merchant;
        }

        private async Task welcomeMerchant(Merchant merchant)
        {
            var folderName = Path.Combine("Resources", "EmailTemplate");
            var pathToEmailTemplate = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            using (StreamReader sr = new StreamReader(pathToEmailTemplate + "/WelcomeMerchant.html"))
            {
                var line = await sr.ReadToEndAsync();

                await _mailer.SendMailAsync(merchant.OwnerEmail, merchant.OwnerName, "Airand: Welcome Onboard", line);
            }
        }
    }
}