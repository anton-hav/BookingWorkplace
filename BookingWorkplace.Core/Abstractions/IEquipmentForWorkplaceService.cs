using BookingWorkplace.Core.DataTransferObjects;

namespace BookingWorkplace.Core.Abstractions;

public interface IEquipmentForWorkplaceService
{
    //READ
    Task<EquipmentForWorkplaceDto> GetEquipmentForWorkplaceByIdAsync(Guid id);
    PagedList<EquipmentForWorkplaceDto> GetEquipmentForWorkplaceByQueryStringParameters(IQueryStringParameters parameters);
    Task<List<EquipmentForWorkplaceDto>> GetAvailableEquipmentForWorkplaceByWorkplaceId(Guid id);
    Task<bool> IsEquipmentForWorkplaceExistAsync(string typeName);
    Task<bool> IsPossibleToFindNecessaryEquipmentToMoveAsync(IFilterParameters parameters);
    Task<List<EquipmentForWorkplaceDto>> GetMovableEquipmentForWorkplaceAsync(IFilterParameters parameters);

    //CREATE
    Task<int> CreateEquipmentForWorkplaceAsync(EquipmentForWorkplaceDto dto);

    //UPDATE
    Task<int> UpdateAsync(Guid id, EquipmentForWorkplaceDto equipment);
    Task PrepareEquipmentForRelocationToWorkplaceAsync(List<EquipmentForWorkplaceDto> equipment, Guid workplaceId);

    //REMOVE
    Task<int> DeleteAsync(Guid id);
}