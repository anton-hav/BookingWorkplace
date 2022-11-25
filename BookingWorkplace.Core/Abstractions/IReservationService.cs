using BookingWorkplace.Core;
using BookingWorkplace.Core.Abstractions;
using BookingWorkplace.Core.DataTransferObjects;

namespace BookingReservation.Core.Abstractions;

public interface IReservationService
{
    //READ
    Task<ReservationDto> GetReservationByIdAsync(Guid id);
    PagedList<ReservationDto> GetReservationsByQueryStringParameters(IQueryStringParameters parameters);
    Task<bool> IsReservationExistAsync(Guid workplaceId, DateTime timeFrom, DateTime timeTo);
    Task<bool> IsReservationForUserExistAsync(Guid userId, DateTime timeFrom, DateTime timeTo);

    //CREATE
    Task<int> CreateReservationAsync(ReservationDto dto);

    //UPDATE
    Task<int> UpdateAsync(Guid id, ReservationDto dto);

    //REMOVE
    Task<int> DeleteAsync(Guid id);
}