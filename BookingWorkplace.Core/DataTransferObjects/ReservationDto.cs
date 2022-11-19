namespace BookingWorkplace.Core.DataTransferObjects;

public class ReservationDto
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }
    public EmployeeDto Employee { get; set; }

    public Guid WorkplaceId { get; set; }
    public WorkplaceDto Workplace { get; set; }

    public DateTime TimeFrom { get; set; }
    public DateTime TimeTo { get; set; }
}