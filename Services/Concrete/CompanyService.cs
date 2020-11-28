using AirandWebAPI.Services.Contract;
using AirandWebAPI.Core;
using AirandWebAPI.Core.Domain;
using System;
using Microsoft.Extensions.Options;
using AirandWebAPI.Helpers;
using System.Threading.Tasks;
using AirandWebAPI.Models.Company;
using System.Transactions;

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

        public async Task<bool> AddDispatchManagers(AddDispatchManagerVM model, int UserId)
        {
            bool response = false;
            DispatchManager dispatchManager;

            var company = _unitOfWork.Companies.SingleOrDefault(x => x.UserId.Equals(UserId));
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                foreach (var item in model.managerDetails)
                {
                    dispatchManager = new DispatchManager();
                    dispatchManager.CompanyId = company.Id;
                    dispatchManager.Name = item.Name;
                    dispatchManager.Email = item.Email;
                    dispatchManager.Phone = item.Phone;
                    dispatchManager.DateCreated = DateTime.Now;
                    _unitOfWork.DispatchManagers.Add(dispatchManager);
                }
                await _unitOfWork.Complete();
                scope.Complete();
                response = true;
            }

            return response;
        }
    }
}