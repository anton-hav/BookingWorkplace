namespace BookingWorkplace.DataBase.Entities;

public class Reservation : IBaseEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }

    public Guid WorkplaceId { get; set; }
    public Workplace Workplace { get; set; }

    public DateTime TimeFrom { get; set; }
    public DateTime TimeTo { get; set; }
}