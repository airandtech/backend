using System.Collections.Generic;
using System.Threading.Tasks;
using AirandWebAPI.Core.Domain;

namespace AirandWebAPI.Services.Contract
{
    public interface IMerchantService
    {
        List<Merchant> GetAll();
        Merchant GetById(int id);
        Task<Merchant> Create(Merchant merchant);
    }

}