using BookingWorkplace.Core.DataTransferObjects;
using BookingWorkplace.Core;

namespace BookingWorkplace.Models;

public class EquipmentDetailModel
{
    public Guid Id { get; set; }
    public string Type { get; set; }

    public PagedList<EquipmentForWorkplaceDto> EquipmentForWorkplaces { get; set; }
}