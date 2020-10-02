using System.ComponentModel.DataAnnotations;

namespace AirandWebAPI.Models.Auth
{
    public class RegisterModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}