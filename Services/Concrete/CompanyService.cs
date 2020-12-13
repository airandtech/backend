using System.Linq;
using AirandWebAPI.Services.Contract;
using AirandWebAPI.Core;
using AirandWebAPI.Core.Domain;
using System;
using Microsoft.Extensions.Options;
using AirandWebAPI.Helpers;
using System.Threading.Tasks;
using AirandWebAPI.Models.Company;
using System.Transactions;
using AirandWebAPI.Models.Auth;
using AutoMapper;
using AirandWebAPI.Models.DTOs;
using System.Collections.Generic;

namespace AirandWebAPI.Services.Concrete
{

    public class CompanyService : ICompanyService
    {
        private IUnitOfWork _unitOfWork;
        private readonly AppSettings _appSettings;
        private IUserService _userService;
        private IMapper _mapper;

        public CompanyService(IUnitOfWork unitOfWork,
        IOptions<AppSettings> appSettings,
        IUserService userService,
        IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _appSettings = appSettings.Value;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<Company> Create(Company company, int UserId)
        {
            var prevCompany = _unitOfWork.Companies.Find(x => x.UserId.Equals(UserId));
            if (prevCompany.Count() > 0)
            {
                return null;
            }

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

        public async Task<bool> AddRiders(AddRidersVM model, int UserId)
        {
            bool response = false;
            Rider rider;
            User user;

            var company = _unitOfWork.Companies.SingleOrDefault(x => x.UserId.Equals(UserId));
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                foreach (var item in model.ridersDetails)
                {
                    user = new User();
                    user.Username = item.Phone;
                    user.Phone = item.Phone;
                    user.FirstName = item.Name;
                    user.CreatedBy = UserId;
                    user.Role = Role.Rider;
                    AuthenticateResponse authResponse = await _userService.Create(user, item.Phone);

                    var existingRider = _unitOfWork.Riders.SingleOrDefault(x => x.UserId.Equals(authResponse.Id));
                    if (existingRider != null)
                        continue;

                    rider = new Rider();
                    rider.UserId = authResponse.Id;
                    rider.CreatedBy = UserId;
                    rider.DateCreated = DateTime.UtcNow.AddHours(1);
                    _unitOfWork.Riders.Add(rider);
                }
                await _unitOfWork.Complete();
                scope.Complete();
                response = true;
            }

            return response;
        }

        public async Task<CompanyWithDetailsVM> CreateCompanyWithDetails(CompanyWithDetailsVM model, int UserId)
        {
            CompanyWithDetailsVM companyWithDetails = null;

            var company = _mapper.Map<Company>(model.company);
            company = await this.Create(company, UserId);
            if (company != null)
            {
                companyWithDetails = model;
                AddRidersVM addRidersVM = new AddRidersVM();
                addRidersVM.ridersDetails = model.ridersDetails;
                var isRiderAdded = await this.AddRiders(addRidersVM, UserId);

                AddDispatchManagerVM addDispatchManager = new AddDispatchManagerVM();
                addDispatchManager.managerDetails = model.managerDetails;
                var isManagerAdded = await this.AddDispatchManagers(addDispatchManager, UserId);
            }
            return companyWithDetails;
        }

        public UserCompanyRider GetCompanyDetails(int UserId)
        {
            UserCompanyRider userCompanyRider = new UserCompanyRider();
            // this is not my best code.. I am dozing like this
            var user = _unitOfWork.Users.Get(UserId);
            if (user != null)
            {
                var userDto = _mapper.Map<UserDto>(user);
                userCompanyRider.userDto = userDto;

                var company = _unitOfWork.Companies.Find(x => x.UserId.Equals(user.Id)).FirstOrDefault();
                if (company != null)
                    userCompanyRider.company = company;

                var riders = _unitOfWork.Users.Find(x => x.CreatedBy.Equals(UserId) && x.Role.Equals(Role.Rider)).ToList();
                if (user != null){

                    userCompanyRider.riders = _mapper.Map<IEnumerable<UserDto>>(riders);
                }
                    
            }

            return userCompanyRider;
        }
    }
}