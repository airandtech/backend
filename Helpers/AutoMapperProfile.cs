using AirandWebAPI.Core.Domain;
using AirandWebAPI.Models.Auth;
using AirandWebAPI.Models.Company;
using AirandWebAPI.Models.DTOs;
using AirandWebAPI.Models.Merchant;
using AutoMapper;

namespace AirandWebAPI.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterModel, User>();
            CreateMap<CreateCompanyVM, Company>();
            CreateMap<User, UserDto>();
            CreateMap<Order, OrderDto>();
            CreateMap<DispatchManager, DispatchManagerDto>();
            CreateMap<RegisterMerchantVM, Merchant>();
        }
    }
}