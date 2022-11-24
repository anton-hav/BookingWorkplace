using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net;
using System.Runtime.CompilerServices;
using Azure;
using BookingWorkplace.Core.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookingWorkplace.Models;

public class ReservationModel 
{
    public Guid Id { get; set; }

    [Required]
    public Guid WorkplaceId { get; set; }

    public WorkplaceDto Workplace { get; set; }

    [Required]
    public DateTime TimeFrom { get; set; }
    [Required]
    //[Remote("CheckReservationForExistence", "Reservation",
    //    AdditionalFields = nameof(WorkplaceId)
    //                       + "," + nameof(TimeFrom)
    //                       + "," + nameof(Id)
    //                       + "," + nameof(UserId),
    //    HttpMethod = WebRequestMethods.Http.Post,
    //    ErrorMessage = "It is not possible to reserve this job for the specified time.")]
    public DateTime TimeTo { get; set; }

    //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    //{
    //    var result = TimeTo < TimeFrom || TimeFrom < DateTime.Today;
    //    if (result)
    //        yield return new ValidationResult("Incorrect time interval selected.");
    //}

    public Dictionary<string, string> ToDictionary()
    {
        return new Dictionary<string, string>
        {
            { nameof(Id), Id.ToString("D") },
            { nameof(WorkplaceId), WorkplaceId.ToString("D") },
            { nameof(TimeFrom), TimeFrom.ToString("u") },
            { nameof(TimeTo), TimeTo.ToString("u") },
        };
    }
}