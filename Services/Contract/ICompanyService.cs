using System.Collections.Generic;
using System.Threading.Tasks;
using AirandWebAPI.Core.Domain;
using AirandWebAPI.Models.Company;
using AirandWebAPI.Models.Direction;

namespace AirandWebAPI.Services.Contract
{


    public interface ICompanyService
    {
        Task<Company> Create(Company company, int UserId);
        Task<bool> AddDispatchManagers(AddDispatchManagerVM model, int UserId);
        Task<bool> AddRiders(AddRidersVM model, int UserId);
        Task<CompanyWithDetailsVM> CreateCompanyWithDetails(CompanyWithDetailsVM company, int UserId);
        UserCompanyRider GetCompanyDetails(int UserId);
        Task<bool> DeleteRider(int riderId);
        DashboardStatisticsVM GetDashboardStatistics(int userId);
    }

}