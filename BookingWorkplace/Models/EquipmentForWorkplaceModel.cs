using System.ComponentModel.DataAnnotations;
using BookingWorkplace.DataBase.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookingWorkplace.Models;

public class EquipmentForWorkplaceModel
{
    public Guid Id { get; set; }

    [Required] public Guid EquipmentId { get; set; }
    public Equipment Equipment { get; set; }
    [Required] public Guid WorkplaceId { get; set; }

    public SelectList AvailableEquipmentList { get; set; }
}