using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace BookingWorkplace.Models;

public class WorkplaceModel
{
    public Guid Id { get; set; }
    [Required]
    [MinLength(1)]
    public string Floor { get; set; }
    [Required]
    [MinLength(1)]
    public string Room { get; set; }
    [Required]
    [MinLength(1)]
    [Remote("CheckWorkplaceForExistence", "Workplace",
        AdditionalFields = nameof(Floor)
                           + "," + nameof(Room),
        HttpMethod = WebRequestMethods.Http.Post,
        ErrorMessage = "The workplace with the same parameters is existence in the database.")]
    public string DeskNumber { get; set; }
}