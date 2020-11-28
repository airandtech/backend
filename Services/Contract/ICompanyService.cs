using System.Collections.Generic;
using System.Threading.Tasks;
using AirandWebAPI.Core.Domain;
using AirandWebAPI.Models.Direction;

namespace AirandWebAPI.Services.Contract
{


    public interface ICompanyService
    {
        Task<Company> Create(Company company, int UserId);
    }

}