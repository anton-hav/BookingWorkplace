namespace BookingWorkplace.DataBase.Entities;

public class EquipmentForWorkplace : IBaseEntity
{
    public Guid Id { get; set; }

    public Guid EquipmentId { get; set; }
    public Equipment Equipment { get; set; }

    public Guid WorkplacesId { get; set; }
    public Workplace Workplace { get; set; }

    public int Count {get; set; }
}