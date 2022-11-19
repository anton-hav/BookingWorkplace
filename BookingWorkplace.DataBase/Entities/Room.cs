namespace BookingWorkplace.DataBase.Entities;

public class Room : IBaseEntity
{
    public Guid Id { get; set; }
    public int Number { get; set; }
    public int Floor { get; set; }

    public List<Workplace> Workplaces { get; set; }
}