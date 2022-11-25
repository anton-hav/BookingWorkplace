namespace BookingWorkplace.SessionUtils.Abstractions;

public interface ISessionManager
{
    // READ
    Task<bool> IsSessionExistAsync();
    Task<ReservationSession> GetSessionAsync();

    // CREATE/UPDATE
    Task SetSessionAsync(ReservationSession session);

    // DELETE
}