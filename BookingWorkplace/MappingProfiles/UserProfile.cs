using AutoMapper;
using BookingWorkplace.Core.DataTransferObjects;
using BookingWorkplace.DataBase.Entities;
using BookingWorkplace.Models;

namespace BookingWorkplace.MappingProfiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>().ForMember(dto => dto.Role,
            opt
                => opt.MapFrom(entity => entity.Role));

        CreateMap<UserDto, User>()
            .ForMember(ent => ent.Id,
                opt
                    => opt.MapFrom(dto => Guid.NewGuid()));

        CreateMap<RegisterModel, UserDto>();

        CreateMap<LoginModel, UserDto>();

        CreateMap<UserDto, UserDataModel>();
    }
}