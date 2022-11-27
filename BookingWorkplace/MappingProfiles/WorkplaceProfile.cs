using AutoMapper;
using BookingWorkplace.Business;
using BookingWorkplace.Core;
using BookingWorkplace.Core.DataTransferObjects;
using BookingWorkplace.DataBase.Entities;
using BookingWorkplace.Models;

namespace BookingWorkplace.MappingProfiles;

public class WorkplaceProfile : Profile
{
    public WorkplaceProfile()
    {
        CreateMap<Workplace, WorkplaceDto>();
        CreateMap<WorkplaceDto, Workplace>();

        CreateMap<WorkplaceDto, WorkplaceModel>();
        CreateMap<WorkplaceModel, WorkplaceDto>();

        CreateMap<WorkplaceDto, WorkplaceWithEquipmentModel>()
            .ForMember(model => model.EquipmentForWorkplaces,
                opt
                    => opt.MapFrom(dto
                        => PagedList<EquipmentForWorkplaceDto>
                            .ToPagedList(dto.EquipmentForWorkplaces.AsQueryable(), new PaginationParameters())));
        CreateMap<WorkplaceWithEquipmentModel, WorkplaceDto>();
    }
}