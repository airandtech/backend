using System.ComponentModel.DataAnnotations;

namespace AirandWebAPI.Models.Auth
{
    public class VerifyPhoneModel
    {
        public string Phone { get; set; }
        public string Otp { get; set; }
    }
}