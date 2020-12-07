using System.ComponentModel.DataAnnotations;

namespace AirandWebAPI.Models.Auth
{
    public class AuthenticateRequest
    {
        //public AuthenticateRequest(){}
        
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public AuthenticateRequest(string username, string password){
            this.Username = username;
            this.Password = password;
        }
    }
}