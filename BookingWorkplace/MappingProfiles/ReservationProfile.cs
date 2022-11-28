using AutoMapper;
using BookingWorkplace.Core.DataTransferObjects;
using BookingWorkplace.DataBase.Entities;
using BookingWorkplace.Models;

namespace BookingWorkplace.MappingProfiles;

public class ReservationProfile : Profile
{
    public ReservationProfile()
    {
        CreateMap<Reservation, ReservationDto>();
        CreateMap<ReservationDto, Reservation>()
            .ForMember(ent => ent.TimeFrom,
                opt
                    => opt.MapFrom(dto => dto.TimeFrom));

        CreateMap<ReservationDto, ReservationModel>();
        CreateMap<ReservationModel, ReservationDto>();
    }
}