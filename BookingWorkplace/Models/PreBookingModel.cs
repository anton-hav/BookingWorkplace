using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using BookingWorkplace.Business;
using BookingWorkplace.Core.Abstractions;
using BookingWorkplace.Core.DataTransferObjects;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;

namespace BookingWorkplace.Models;

public class PreBookingModel //: IValidatableObject
{
    //[Required]
    //public DateTime TimeFrom { get; set; }
    //[Required]
    //public DateTime TimeTo { get; set; }
    
    public FilterParameters Filters { get; set; }
    
    public List<SelectListItem> EquipmentList { get; set; }

    public List<WorkplaceDto>? Workplaces { get; set; }

    public List<WorkplaceDto>? UnderstaffedWorkplaces {get; set; }

    //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    //{
    //    var result = TimeTo < TimeFrom || TimeFrom < DateTime.Today;
    //    if (result)
    //        yield return new ValidationResult("Incorrect time interval selected.");
    //}
}