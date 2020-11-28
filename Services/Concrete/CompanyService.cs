using AirandWebAPI.Services.Contract;
using AirandWebAPI.Core;
using AirandWebAPI.Core.Domain;
using System;
using Microsoft.Extensions.Options;
using AirandWebAPI.Helpers;
using System.Threading.Tasks;

namespace AirandWebAPI.Services.Concrete
{

    public class CompanyService : ICompanyService
    {
        private IUnitOfWork _unitOfWork;
        private readonly AppSettings _appSettings;

        public CompanyService(IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings)
        {
            _unitOfWork = unitOfWork;
            _appSettings = appSettings.Value;
        }

        public async Task<Company> Create(Company company, int UserId)
        {
            company.UserId = UserId;
            company.DateCreated = DateTime.Now;
            _unitOfWork.Companies.Add(company);
            await _unitOfWork.Complete();
            return company;
        }
    }
}