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
using System.Net.Http;
using System.Net.Http.Headers;
using AirandWebAPI.Models;
using System.Text;
using System.Net;
using Newtonsoft.Json;

namespace AirandWebAPI.Services.Concrete
{

    public class CompanyService : ICompanyService
    {
        private readonly string apiKey = "FLWSECK-891e490e3196c2de0debb041ed1eb6dd-X";
        private IUnitOfWork _unitOfWork;
        private readonly AppSettings _appSettings;
        private IUserService _userService;
        private IOrderService _orderService;
        private IMapper _mapper;

        public CompanyService(IUnitOfWork unitOfWork,
        IOptions<AppSettings> appSettings,
        IUserService userService,
        IOrderService orderService,
        IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _appSettings = appSettings.Value;
            _userService = userService;
            _orderService = orderService;
            _mapper = mapper;
        }

        public List<Company> GetAll()
        {
            return _unitOfWork.Companies.GetAll().ToList();
        }

        public Company GetById(int id)
        {
            return _unitOfWork.Companies.Get(id);
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

                    int userId;
                    if (authResponse == null)
                    {
                        userId = _unitOfWork.Users.SingleOrDefault(x => x.Phone.Equals(item.Phone)).Id;
                    }
                    else
                    {
                        userId = authResponse.Id;
                    }

                    var existingRider = _unitOfWork.Riders.SingleOrDefault(x => x.UserId.Equals(userId));
                    if (existingRider != null)
                        continue;

                    rider = new Rider();
                    rider.UserId = userId;
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

            var existingCompany = _unitOfWork.Companies.SingleOrDefault(x => x.UserId.Equals(UserId));
            if (existingCompany != null)
            {
                var company = this.updateCompany(existingCompany, model.company);
                var user = this.updateManager(existingCompany.Id, model.managerDetails);
                return model;
            }
            else
            {

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
                {
                    userCompanyRider.company = company;

                    var managers = _unitOfWork.DispatchManagers.Find(x => x.CompanyId.Equals(company.Id)).ToList();
                    if (managers != null && managers.Count() > 0)
                    {
                        var managerDto = _mapper.Map<IEnumerable<DispatchManagerDto>>(managers);
                        userCompanyRider.managers = managerDto;
                    }
                }

                var riders = _unitOfWork.Riders.GetAllRidersWithUsers().Where(x => x.CreatedBy.Equals(UserId)).ToList();
                if (riders != null && riders.Count() > 0)
                {
                    userCompanyRider.riders = riders;
                }

            }

            return userCompanyRider;
        }

        public async Task<bool> DeleteRider(int riderId)
        {
            Rider rider = _unitOfWork.Riders.Get(riderId);
            if (rider != null)
            {
                int userId = rider.UserId;
                User user = _unitOfWork.Users.Get(userId);
                if (user != null)
                {
                    _unitOfWork.Riders.Remove(rider);
                    _unitOfWork.Users.Remove(user);
                    await _unitOfWork.Complete();
                    return true;
                }
            }

            return false;
        }
        public DashboardStatisticsVM GetDashboardStatistics(int userId)
        {
            DashboardStatisticsVM dashboardStatistics = new DashboardStatisticsVM();
            var ridersWithUsers = _unitOfWork.Riders.GetAllRidersWithUsers().Where(x => x.CreatedBy.Equals(userId)).OrderByDescending(x => x.LastModified).Take(5).ToList();
            int[] ridersId = ridersWithUsers.Select(x => x.Id).ToArray();
            var allOrders = _unitOfWork.Orders.Find(x => x.RiderId != null).ToList();

            dashboardStatistics.riders = ridersWithUsers;

            if (allOrders.Any() && ridersId.Count() > 0)
            {
                var orders = allOrders.Where(x => ((IList<int>)ridersId).Contains(int.Parse(x.RiderId))).ToList();

                // dashboardStatistics.riders = ridersWithUsers;
                dashboardStatistics.todayTransactions = orders.Where(x => x.LastModified.Date.Equals(DateTime.Now.Date)).ToList();
                dashboardStatistics.todayTransactionsVolume = dashboardStatistics.todayTransactions.Count();
                dashboardStatistics.todayTransactionsValue = dashboardStatistics.todayTransactions.Sum(item => item.Cost);
                dashboardStatistics.totalTransactions = orders.ToList();
                dashboardStatistics.totalTransactionsValue = orders.Sum(x => x.Cost);
                dashboardStatistics.todayTransactionsVolume = orders.Count();
                var successfulTransactions = orders.Where(x => x.PaymentStatus.Equals(1));
                dashboardStatistics.totalSuccessfulVolume = successfulTransactions.Count();
                dashboardStatistics.totalSuccessValue = successfulTransactions.Sum(x => x.Cost);
                dashboardStatistics.orders = _orderService.GetOrdersForCompany(10, 0, userId);

            }

            return dashboardStatistics;
        }

        public async Task<ResolveAccountResponseVM> ResolveAccount(ResolveAccountVM model)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.flutterwave.com/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            ResolveAccountRequest jsonObject = new ResolveAccountRequest(model.AccountNumber, model.BankCode);

            string json = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);

            var body = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsJsonAsync("v3/accounts/resolve", jsonObject);
            //response.EnsureSuccessStatusCode();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                ResolveAccountRootResponse resolveResponse = JsonConvert.DeserializeObject<ResolveAccountRootResponse>(responseBody);
                if (resolveResponse.status.Equals("success"))
                {
                    return resolveResponse.data;
                }
            }

            return null;
        }
        private Company updateCompany(Company existingCompany, CreateCompanyVM update)
        {

            if (!string.IsNullOrWhiteSpace(update.AccountName))
            {
                existingCompany.AccountName = update.AccountName;
            }
            if (!string.IsNullOrWhiteSpace(update.AccountNumber))
            {
                existingCompany.AccountNumber = update.AccountNumber;
            }
            if (!string.IsNullOrWhiteSpace(update.CompanyAddress))
            {
                existingCompany.CompanyAddress = update.CompanyAddress;
            }
            if (!string.IsNullOrWhiteSpace(update.BankName))
            {
                existingCompany.BankName = update.BankName;
            }
            if (!string.IsNullOrWhiteSpace(update.CompanyName))
            {
                existingCompany.CompanyName = update.CompanyName;
            }
            if (!string.IsNullOrWhiteSpace(update.OfficeArea))
            {
                existingCompany.OfficeArea = update.OfficeArea;
            }

            var resp = _unitOfWork.Complete();
            return existingCompany;
        }
        private DispatchManager updateManager(int companyId, IEnumerable<ManagerDetails> managerDetails)
        {
            var existingManager = _unitOfWork.DispatchManagers.Find(x => x.CompanyId.Equals(companyId)).FirstOrDefault();
            var manager = managerDetails.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(manager.Name))
            {
                existingManager.Name = manager.Name;
            }
            if (!string.IsNullOrWhiteSpace(manager.Email))
            {
                existingManager.Email = manager.Email;
            }
            if (!string.IsNullOrWhiteSpace(manager.Phone))
            {
                existingManager.Phone = manager.Phone;
            }
            var resp = _unitOfWork.Complete();
            return existingManager;
        }
    }
}