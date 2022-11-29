using BookingWorkplace.Business;
using BookingWorkplace.Core.Abstractions;

namespace BookingWorkplace.SessionUtils.Abstractions;

public interface ISessionManager
{
    // READ
    Task<bool> IsSessionExistAsync();
    Task<ReservationSession> GetSessionAsync();

    // CREATE/UPDATE
    Task SetSessionAsync(ReservationSession session);
    Task <FilterParameters> SynchronizeFilterAndSessionAsync(FilterParameters filters);
    Task CreateNewReservationSessionAsync(FilterParameters filters);

    // DELETE
    Task RemoveSessionAsync();
}