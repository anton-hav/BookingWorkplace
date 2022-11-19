namespace BookingWorkplace.DataBase.Entities;

public class Workplace : IBaseEntity
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public int DeskNumber { get; set; }

    public List<Reservation> Reservations { get; set; }
    public List<EquipmentForWorkplace> EquipmentForWorkplaces { get; set; }
}