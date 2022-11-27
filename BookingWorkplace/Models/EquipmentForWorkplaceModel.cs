using BookingWorkplace.Core.DataTransferObjects;
using BookingWorkplace.DataBase.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BookingWorkplace.Models;

public class EquipmentForWorkplaceModel
{
    public Guid Id { get; set; }

    [Required]
    public Guid EquipmentId { get; set; }
    public Equipment Equipment { get; set; }
    [Required]
    public Guid WorkplaceId { get; set; }

    public SelectList AvailableEquipmentList { get; set; }
}