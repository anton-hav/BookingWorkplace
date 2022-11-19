namespace BookingWorkplace.Core.DataTransferObjects;

public class WorkplaceDto
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public int DeskNumber { get; set; }

    public List<ReservationDto> Reservations { get; set; }
    public List<EquipmentForWorkplaceDto> EquipmentForWorkplaces { get; set; }
}