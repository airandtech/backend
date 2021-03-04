using System.Threading.Tasks;
using AirandWebAPI.Core.Domain;

namespace AirandWebAPI.Services.Contract
{
    public interface IMerchantService
    {
        Task<Merchant> Create(Merchant merchant);
    }

}