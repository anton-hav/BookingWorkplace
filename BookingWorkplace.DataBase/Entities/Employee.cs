namespace BookingWorkplace.DataBase.Entities;

public class Employee : IBaseEntity
{
    public Guid Id { get; set; }
    public string FullName { get; set; }

    public List<Reservation> Reservations { get; set; }
}