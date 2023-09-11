
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

      // Update FROM MessageUser -> MessageUser dto

       CreateMap<MessageUser, MessageUserDto>()
      .ForMember(dest => dest.messageSenderPhotoURL, opt => opt.MapFrom(src => src.SenderUser.Photos.FirstOrDefault(x => x.IsMainPhoto).Url)) // get the main photo of the sender
      .ForMember(dest => dest.messageReceivingPhotoURL, opt => opt.MapFrom(src => src.ReceivingUser.Photos.FirstOrDefault(x => x.IsMainPhoto).Url)); // get the main photo of the receiving

      
      // Below is individual mapping for each property (if needed to separate in future)

      // Sender -> Receiving
      // CreateMap<MessageUser, MessageUserDto>()
      // .ForMember(dest => dest.messageSenderPhotoURL, opt => opt.MapFrom(src => src.SenderUser.Photos.FirstOrDefault(x => x.IsMainPhoto).Url)); // get the main photo of the sender

      // Receiving -> Sender
      // CreateMap<MessageUser, MessageUserDto>()
      // .ForMember(dest => dest.messageReceivingPhotoURL, opt => opt.MapFrom(src => src.ReceivingUser.Photos.FirstOrDefault(x => x.IsMainPhoto).Url)); // get the main photo of the receiving

     

      // ^ Automapper can figure out the rest of the properties, but for the photo, we need to specify the source and destination 

    }

  }
}