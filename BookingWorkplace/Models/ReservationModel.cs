using System.ComponentModel.DataAnnotations;
using BookingWorkplace.Core.DataTransferObjects;

namespace BookingWorkplace.Models;

public class ReservationModel
{
    public Guid Id { get; set; }

    [Required] public Guid WorkplaceId { get; set; }

    public WorkplaceDto Workplace { get; set; }

    [Required] public DateTime TimeFrom { get; set; }

    [Required] public DateTime TimeTo { get; set; }

    //public Dictionary<string, string> ToDictionary()
    //{
    //    return new Dictionary<string, string>
    //    {
    //        { nameof(Id), Id.ToString("D") },
    //        { nameof(WorkplaceId), WorkplaceId.ToString("D") },
    //        { nameof(TimeFrom), TimeFrom.ToString("u") },
    //        { nameof(TimeTo), TimeTo.ToString("u") }
    //    };
    //}
}