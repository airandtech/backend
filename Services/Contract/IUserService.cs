using System.Collections.Generic;
using AirandWebAPI.Core.Domain;
using AirandWebAPI.Models;

namespace AirandWebAPI.Services{


    public interface IUserService
    {
        User GetById(int id);
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        IEnumerable<User> GetAll();
    }

}