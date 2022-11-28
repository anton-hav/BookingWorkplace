using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace BookingWorkplace.Models;

public class EquipmentModel
{
    public Guid Id { get; set; }

    [Required]
    [MinLength(2)]
    [Remote("CheckEquipmentForExistence", "Equipment",
        HttpMethod = WebRequestMethods.Http.Post,
        ErrorMessage = "The equipment with the same type is existence in the database.")]
    public string Type { get; set; }
}