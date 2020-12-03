using System;
using AirandWebAPI.Models.DTOs;
using AirandWebAPI.Core;
using System.Collections.Generic;
using AirandWebAPI.Core.Domain;

namespace AirandWebAPI.Models.Company
{
    public class UserCompanyRider
    {
        public UserDto userDto {get;set;}
        public Core.Domain.Company company {get;set;}
        public IEnumerable<Rider> riders {get;set;}
    }
}