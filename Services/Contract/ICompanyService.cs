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
    }

}