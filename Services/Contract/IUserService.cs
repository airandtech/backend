using System.Collections.Generic;
using System.Threading.Tasks;
using AirandWebAPI.Core.Domain;
using AirandWebAPI.Models.Auth;

namespace AirandWebAPI.Services.Contract{


    public interface IUserService
    {
        User GetById(int id);
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        IEnumerable<User> GetAll();
        Task<AuthenticateResponse> Create(User user, string password);
        Task<bool> CheckPhone(string phone);
        Task<bool> VerifyPhone(VerifyPhoneModel model);
    }

}