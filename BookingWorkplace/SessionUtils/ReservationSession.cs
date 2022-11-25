namespace BookingWorkplace.SessionUtils;

public class ReservationSession
{
    public Guid ReservationId { get; set; }
    // todo: remove it
    public Guid UserId { get; set; }
    // todo: remove it
    public Guid WorkplaceId { get; set; }
    public List<Guid> EquipmentIds { get; set; }
    public DateTime TimeFrom { get; set; }
    public DateTime TimeTo { get; set; }
}