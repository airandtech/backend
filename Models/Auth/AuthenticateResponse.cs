using AirandWebAPI.Core.Domain;

namespace AirandWebAPI.Models.Auth
{
    public class AuthenticateResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
        public bool IsSetupComplete { get; set; }


        public AuthenticateResponse(User user, string token, bool isSetupComplete)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Username = user.Username;
            Token = token;
            IsSetupComplete = isSetupComplete;
            Role = user.Role;
        }
    }
}