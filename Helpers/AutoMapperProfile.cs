using AirandWebAPI.Core.Domain;
using AirandWebAPI.Models.Auth;
using AutoMapper;

namespace AirandWebAPI.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterModel, User>();
        }
    }
}