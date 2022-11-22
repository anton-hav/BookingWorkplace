namespace BookingWorkplace.DataBase.Entities;

public class User : IBaseEntity
{
    public Guid Id { get; set; }

    public string Email { get; set; }
    public string PasswordHash { get; set; }
    
    public string? FullName { get; set; }

    public Guid RoleId { get; set; }
    public Role Role { get; set; }

    public List<Reservation> Reservations { get; set; }
}