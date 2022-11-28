using BookingWorkplace.Core.Abstractions;

namespace BookingWorkplace.SessionUtils;

public class ReservationSession : IFilterParameters
{
    public Guid ReservationId { get; set; }

    // todo: remove it
    public Guid UserId { get; set; }

    // todo: remove it
    public Guid WorkplaceId { get; set; }
    public List<Guid> EquipmentIds { get; set; }
    public DateTime TimeFrom { get; set; }
    public DateTime TimeTo { get; set; }

    /// <summary>
    ///     Convert ReservationSession object to the dictionary.
    /// </summary>
    /// <returns><see cref="IDictionary{TKey,TValue}" /> where TKey - property name, TValue - property value</returns>
    /// <exception cref="NotImplementedException"></exception>
    public IDictionary<string, string> ToDictionary()
    {
        throw new NotImplementedException("ToDictionary method should not be used for the session object.");
    }
}