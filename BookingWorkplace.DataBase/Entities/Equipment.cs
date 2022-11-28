namespace BookingWorkplace.DataBase.Entities;

public class Equipment : IBaseEntity
{
    public Guid Id { get; set; }
    public string Type { get; set; }

    public List<EquipmentForWorkplace> EquipmentForWorkplaces { get; set; }
}