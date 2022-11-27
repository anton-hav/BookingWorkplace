using BookingWorkplace.Business;
using BookingWorkplace.Core.DataTransferObjects;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookingWorkplace.Models;

public class PreBookingModel
{
    public FilterParameters Filters { get; set; }

    public List<SelectListItem> EquipmentList { get; set; }

    public List<WorkplaceDto>? Workplaces { get; set; }

    public List<WorkplaceDto>? UnderstaffedWorkplaces { get; set; }
}