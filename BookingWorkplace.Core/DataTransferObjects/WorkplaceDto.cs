namespace BookingWorkplace.Core.DataTransferObjects;

public class WorkplaceDto
{
    public Guid Id { get; set; }
    public string Floor { get; set; }
    public string Room { get; set; }
    public string DeskNumber { get; set; }

    public List<ReservationDto> Reservations { get; set; }
    public List<EquipmentForWorkplaceDto> EquipmentForWorkplaces { get; set; }
}