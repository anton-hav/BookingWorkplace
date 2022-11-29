using BookingWorkplace.Core.DataTransferObjects;

namespace BookingWorkplace.Core.Abstractions;

public interface IEquipmentForWorkplaceService
{
    //READ
    Task<EquipmentForWorkplaceDto> GetEquipmentForWorkplaceByIdAsync(Guid id);
    Task<EquipmentDto> GetEquipmentByEquipmentForWorkplaceIdAsync(Guid id);
    Task<EquipmentMovementData> GetEquipmentMovementDataAsync(Guid id, Guid destinationId);
    Task<bool> IsPossibleToFindNecessaryEquipmentToMoveAsync(IFilterParameters parameters);

    Task<List<EquipmentForWorkplaceDto>> GetMovableEquipmentForWorkplaceAsync(IFilterParameters parameters,
        Guid workplaceId);

    //CREATE
    Task<int> CreateEquipmentForWorkplaceAsync(EquipmentForWorkplaceDto dto);

    //UPDATE
    Task PrepareEquipmentForRelocationToWorkplaceAsync(List<EquipmentForWorkplaceDto> equipment, Guid workplaceId);

    //REMOVE
    Task<int> DeleteAsync(Guid id);
}