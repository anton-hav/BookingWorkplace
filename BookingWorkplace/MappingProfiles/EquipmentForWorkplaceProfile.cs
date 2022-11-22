using AutoMapper;
using BookingWorkplace.Core.DataTransferObjects;
using BookingWorkplace.DataBase.Entities;
using BookingWorkplace.Models;

namespace BookingWorkplace.MappingProfiles;

public class EquipmentForWorkplaceProfile : Profile
{
    public EquipmentForWorkplaceProfile()
    {
        CreateMap<EquipmentForWorkplace, EquipmentForWorkplaceDto>();
        CreateMap<EquipmentForWorkplaceDto, EquipmentForWorkplace>();

        CreateMap<EquipmentForWorkplaceDto, EquipmentForWorkplaceModel>();
        CreateMap<EquipmentForWorkplaceModel, EquipmentForWorkplaceDto>();
    }
}