
using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.ExternalHelpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AppUser, MemberDto>()
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()))
            .ForMember(dest => dest.photoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMainPhoto).Url));
            CreateMap<Photo, PhotoDto>(); 
            // Update FROM UpdateMember dto -> App User
            CreateMap<UpdateMemberDto, AppUser>();

            // Update FROM Register dto -> App User
            CreateMap<RegisterDto, AppUser>();
        }

    }
}