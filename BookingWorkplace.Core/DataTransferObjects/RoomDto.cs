namespace BookingWorkplace.Core.DataTransferObjects;

public class RoomDto
{
    public Guid Id { get; set; }
    public int Number { get; set; }
    public int Floor { get; set; }

    public List<WorkplaceDto> Workplaces { get; set; }
}