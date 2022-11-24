namespace BookingWorkplace.SessionUtils;

public class ReservationSession
{
    public Guid ReservationId { get; set; }
    public Guid UserId { get; set; }
    public Guid WorkplaceId { get; set; }
    public DateTime TimeFrom { get; set; }
    public DateTime TimeTo { get; set; }
}