using System.ComponentModel.DataAnnotations;
using BookingWorkplace.Core.Abstractions;

namespace BookingWorkplace.Business;

public class FilterParameters : IFilterParameters
{
    public List<Guid> Ids { get; set; }
    public DateTime TimeFrom { get; set; }
    public DateTime TimeTo { get; set; }

    public IDictionary<string, string> ToDictionary()
    {
        var dict = new Dictionary<string, string>
        {
            { nameof(TimeFrom), TimeFrom.ToString("u") },
            { nameof(TimeTo), TimeTo.ToString("u") }
        };

        for (int i = 0; i < Ids.Count; i++)
        {
            dict.Add($"{nameof(Ids)}[{i}]", Ids[i].ToString("N"));
        };

        return dict;
    }

    //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    //{
    //    var result = TimeTo < TimeFrom || TimeFrom < DateTime.Today;
    //    if (result)
    //        yield return new ValidationResult("Incorrect time interval selected.");
    //}
}