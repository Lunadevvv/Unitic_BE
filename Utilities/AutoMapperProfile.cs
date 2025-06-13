
using AutoMapper;
using Unitic_BE.Dtos;
using Unitic_BE.Models;

namespace Unitic_BE.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserResponseDto>();

            CreateMap<RegisterDto, User>();
                //.ForMember(dest => dest.Password, opt => opt.Ignore());

        }
    }
}
