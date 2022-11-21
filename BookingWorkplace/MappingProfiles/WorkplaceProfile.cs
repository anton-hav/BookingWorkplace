using AutoMapper;
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
    }
}