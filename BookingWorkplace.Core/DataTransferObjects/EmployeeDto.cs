namespace BookingWorkplace.Core.DataTransferObjects;

public class EmployeeDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }

    public List<ReservationDto> Reservations { get; set; }
}