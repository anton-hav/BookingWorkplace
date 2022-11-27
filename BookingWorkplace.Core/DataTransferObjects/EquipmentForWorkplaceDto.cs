namespace BookingWorkplace.Core.DataTransferObjects;

public class EquipmentForWorkplaceDto
{
    public Guid Id { get; set; }

    public Guid EquipmentId { get; set; }
    public EquipmentDto Equipment { get; set; }

    public Guid WorkplaceId { get; set; }
    public WorkplaceDto Workplace { get; set; }
}