using BookingWorkplace.Core.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using BookingWorkplace.Core;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookingWorkplace.Models;

public class WorkplaceWithEquipmentModel
{
    public Guid Id { get; set; }
    public string Floor { get; set; }
    public string Room { get; set; }
    public string DeskNumber { get; set; }

    public PagedList<EquipmentForWorkplaceDto> EquipmentForWorkplaces { get; set; }

    public SelectList AvailableEquipmentList {get; set; }
}