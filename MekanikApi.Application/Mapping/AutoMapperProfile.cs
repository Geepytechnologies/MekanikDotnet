using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MekanikApi.Application.DTOs.Auth.Requests;
using MekanikApi.Application.DTOs.Auth.Responses;
using MekanikApi.Application.DTOs.Roles;

namespace MekanikApi.Application.mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            
            CreateMap<IdentityRole<Guid>, RoleRequest>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
        }
    }
}