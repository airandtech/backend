using System;

namespace AirandWebAPI.Models.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public string Gender { get; set; }
        public string ProfilePicture { get; set; }
        public string Bio { get; set; }
        public string Token { get; set; }
        public bool isVerified { get; set; }
        public bool isCompany { get; set; }
        public DateTime DateCreated {get;set;}
        public DateTime LastModified {get;set;}
    }
}