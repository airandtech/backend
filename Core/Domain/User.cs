using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AirandWebAPI.Core.Domain
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public string Gender { get; set; }
        public string ProfilePicture { get; set; }
        public string Bio { get; set; }
        public string Token { get; set; }
        
        [JsonIgnore]
        public string Password {get;set;}
    }
}
