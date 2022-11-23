namespace BookingWorkplace.Core.DataTransferObjects;

public class ReservationDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public UserDto User { get; set; }

    public Guid WorkplaceId { get; set; }
    public WorkplaceDto Workplace { get; set; }

    public DateTime TimeFrom { get; set; }
    public DateTime TimeTo { get; set; }
}