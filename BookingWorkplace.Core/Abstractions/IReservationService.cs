using BookingWorkplace.Core.DataTransferObjects;

namespace BookingWorkplace.Core.Abstractions;

public interface IReservationService
{
    //READ
    Task<ReservationDto> GetReservationByIdAsync(Guid id);
    PagedList<ReservationDto> GetReservationsByQueryStringParameters(IQueryStringParameters parameters);
    PagedList<ReservationDto> GetReservationsByQueryStringParameters(IQueryStringParameters parameters, Guid userId);
    Task<DateTime> GetEndDateOfReservationByWorkplaceId(Guid workplaceId);
    Task<bool> IsReservationExistAsync(Guid workplaceId, DateTime timeFrom, DateTime timeTo);
    Task<bool> IsReservationForUserExistAsync(Guid userId, DateTime timeFrom, DateTime timeTo);

    //CREATE
    Task<int> CreateReservationAsync(ReservationDto dto);

    //UPDATE

    //REMOVE
    Task<int> DeleteAsync(ReservationDto dto);
}