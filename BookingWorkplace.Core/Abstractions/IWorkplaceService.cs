using BookingWorkplace.Core.DataTransferObjects;

namespace BookingWorkplace.Core.Abstractions;

public interface IWorkplaceService
{
    //READ
    Task<WorkplaceDto> GetWorkplaceByIdAsync(Guid id);
    Task<WorkplaceDto> GetWorkplaceWithEquipmentByIdAsync(Guid id);
    PagedList<WorkplaceDto> GetWorkplacesByQueryStringParameters(IQueryStringParameters parameters);
    Task<bool> IsWorkplaceExistAsync(string roomNumber, string floorNumber, string deskNumber);

    //CREATE
    Task<int> CreateWorkplaceAsync(WorkplaceDto dto);

    //UPDATE
    Task<int> UpdateAsync(Guid id, WorkplaceDto dto);

    //REMOVE
    Task<int> DeleteAsync(Guid id);
}