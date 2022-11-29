using BookingWorkplace.Business;
using BookingWorkplace.Core.Abstractions;
using BookingWorkplace.SessionUtils.Abstractions;
using Microsoft.IdentityModel.Tokens;

namespace BookingWorkplace.SessionUtils.MangerImplementations;

public class SessionManager : ISessionManager
{
    private readonly IHttpContextAccessor _contextAccessor;
    private HttpContext _context;
    private readonly IUserManager _userManager;
    private const int SessionKeyOffset = 5;

    public SessionManager(IHttpContextAccessor contextAccessor,
        IUserManager userManager)
    {
        _contextAccessor = contextAccessor;
        _userManager = userManager;
    }

    private HttpContext Context
    {
        get
        {
            var context = _context ?? _contextAccessor?.HttpContext;
            if (context == null) throw new InvalidOperationException("HttpContext must not be null.");
            return context;
        }
        set => _context = value;
    }

    /// <summary>
    ///     Checks the existence of a reservation session for the current user
    /// </summary>
    /// <returns>A Boolean</returns>
    public async Task<bool> IsSessionExistAsync()
    {
        var sessionKey = await GetSessionKeyAsync();
        var isSucceed = Context.Session.TryGetValue<ReservationSession>(sessionKey, out var _);
        return isSucceed;
    }

    /// <summary>
    ///     Gets a reservation session for the current user.
    /// </summary>
    /// <returns>
    ///     <see cref="ReservationSession" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<ReservationSession> GetSessionAsync()
    {
        var sessionKey = await GetSessionKeyAsync();
        var isSucceed = Context.Session.TryGetValue<ReservationSession>(sessionKey, out var reservationSession);
        if (isSucceed)
            return reservationSession;

        throw new InvalidOperationException(
            "Session not found. Possibly the lifetime of the session has been exceeded.");
    }

    /// <summary>
    ///     Sets a reservation session for the current user.
    /// </summary>
    /// <param name="session">a <see cref="ReservationSession" /> instance</param>
    /// <returns>The Task</returns>
    public async Task SetSessionAsync(ReservationSession session)
    {
        var sessionKey = await GetSessionKeyAsync();
        Context.Session.Set(sessionKey, session);
    }

    /// <summary>
    /// Checks and initiates value for date fields of filters
    /// </summary>
    /// <param name="filters"><see cref="FilterParameters"/></param>
    /// <returns><see cref="FilterParameters"/></returns>
    public async Task<FilterParameters> SynchronizeFilterAndSessionAsync(FilterParameters filters)
    {
        var session = await GetSessionAsync();

        if (filters.TimeFrom.Equals(default))
            filters.TimeFrom = session.TimeFrom;
        else
            session.TimeFrom = filters.TimeFrom;

        if (filters.TimeTo.Equals(default))
            filters.TimeTo = session.TimeTo;
        else
            session.TimeTo = filters.TimeTo;

        if (filters.EquipmentIds.IsNullOrEmpty()) filters.EquipmentIds = new List<Guid>();

        session.EquipmentIds = filters.EquipmentIds;

        await SetSessionAsync(session);

        return filters;
    }

    /// <summary>
    /// Create new reservation session
    /// </summary>
    /// <param name="filters"><see cref="FilterParameters"/></param>
    /// <returns>The Task</returns>
    public async Task CreateNewReservationSessionAsync(FilterParameters filters)
    {
        var userId = await _userManager.GetUserIdAsync();
        var reservationSession = new ReservationSession
        {
            ReservationId = Guid.NewGuid(),
            UserId = userId,
            WorkplaceId = Guid.Empty,
            TimeFrom = filters.TimeFrom.Equals(default) ? DateTime.Today : filters.TimeFrom,
            TimeTo = filters.TimeTo.Equals(default) ? DateTime.Today : filters.TimeTo,
            EquipmentIds = filters.EquipmentIds.IsNullOrEmpty() ? new List<Guid>() : filters.EquipmentIds
        };

        await SetSessionAsync(reservationSession);
    }

    /// <summary>
    ///     Remove the current reservation session
    /// </summary>
    /// <returns>The Task</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task RemoveSessionAsync()
    {
        if (!Context.Session.IsAvailable)
            throw new InvalidOperationException(
                "Session not found. Possibly the lifetime of the session has been exceeded.");

        var sessionKey = await GetSessionKeyAsync();
        Context.Session.Remove(sessionKey);
    }

    /// <summary>
    ///     Generates a session key from the current User Id
    /// </summary>
    /// <returns>a session key as a string</returns>
    private async Task<string> GetSessionKeyAsync()
    {
        var userId = await _userManager.GetUserIdAsync();
        var sessionKey = userId.ToString("N").Substring(SessionKeyOffset);
        return sessionKey;
    }
}