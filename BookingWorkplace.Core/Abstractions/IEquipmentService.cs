using BookingWorkplace.Core.DataTransferObjects;

namespace BookingWorkplace.Core.Abstractions;

public interface IEquipmentService
{
    //READ
    Task<EquipmentDto> GetEquipmentByIdAsync(Guid id);
    PagedList<EquipmentDto> GetEquipmentByQueryStringParameters(IQueryStringParameters parameters);
    Task<bool> IsEquipmentExistAsync(string typeName);

    //CREATE
    Task<int> CreateEquipmentAsync(EquipmentDto dto);

    //UPDATE
    Task<int> UpdateAsync(Guid id, EquipmentDto equipment);

    //REMOVE
    Task<int> DeleteAsync(Guid id);
}