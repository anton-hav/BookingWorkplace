namespace BookingWorkplace.DataBase.Entities;

public class Workplace : IBaseEntity
{
    public Guid Id { get; set; }
    public string Floor { get; set; }
    public string Room { get; set; }
    public string DeskNumber { get; set; }

    public List<Reservation> Reservations { get; set; }
    public List<EquipmentForWorkplace> EquipmentForWorkplaces { get; set; }
}