using BookingWorkplace.Core.DataTransferObjects;

namespace BookingWorkplace.Core.Abstractions;

public interface IWorkplaceService
{
    //READ
    Task<WorkplaceDto> GetWorkplaceByIdAsync(Guid id);
    Task<WorkplaceDto> GetWorkplaceWithEquipmentByIdAsync(Guid id);
    PagedList<WorkplaceDto> GetWorkplacesByQueryStringParameters(IQueryStringParameters parameters);
    List<WorkplaceDto> GetWorkplacesByFilterParameters(IFilterParameters parameters);
    Task<List<Guid>> GetListOfMissingEquipment(Guid id, List<Guid> equipmentIds);

    Task<List<WorkplaceDto>> GetPossibleWorkplacesByFilterParameters(IFilterParameters parameters,
        List<WorkplaceDto> exclusionList);

    Task<bool> IsWorkplaceExistAsync(string roomNumber, string floorNumber, string deskNumber);

    //CREATE
    Task<int> CreateWorkplaceAsync(WorkplaceDto dto);

    //UPDATE
    Task<int> UpdateAsync(Guid id, WorkplaceDto dto);

    //REMOVE
}