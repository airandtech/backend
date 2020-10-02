using System.Collections.Generic;
using AirandWebAPI.Core.Domain;
using AirandWebAPI.Models.Auth;

namespace AirandWebAPI.Services.Contract{


    public interface IUserService
    {
        User GetById(int id);
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        IEnumerable<User> GetAll();
        AuthenticateResponse Create(User user, string password);
    }

}