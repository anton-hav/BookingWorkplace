namespace BookingWorkplace.Core.DataTransferObjects;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public string? FullName { get; set; }

    public Guid RoleId { get; set; }
    public RoleDto Role { get; set; }

    public List<ReservationDto> Reservations { get; set; }
}