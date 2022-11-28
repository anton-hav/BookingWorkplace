namespace BookingWorkplace.Core.DataTransferObjects;

public class EquipmentDto
{
    public Guid Id { get; set; }
    public string Type { get; set; }

    public List<EquipmentForWorkplaceDto> EquipmentForWorkplaces { get; set; }
}