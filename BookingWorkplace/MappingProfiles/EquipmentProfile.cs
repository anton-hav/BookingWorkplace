using AutoMapper;
using BookingWorkplace.Core.DataTransferObjects;
using BookingWorkplace.DataBase.Entities;
using BookingWorkplace.Models;

namespace BookingWorkplace.MappingProfiles;

public class EquipmentProfile : Profile
{
    public EquipmentProfile()
    {
        CreateMap<Equipment, EquipmentDto>();
        CreateMap<EquipmentDto, Equipment>();

        CreateMap<EquipmentDto, EquipmentModel>();
        CreateMap<EquipmentModel, EquipmentDto>();

        CreateMap<EquipmentDto, EquipmentDetailModel>();
        CreateMap<EquipmentDetailModel, EquipmentDto>();
    }
}