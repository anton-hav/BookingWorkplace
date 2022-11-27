using System.ComponentModel.DataAnnotations;
using BookingWorkplace.Core.Abstractions;

namespace BookingWorkplace.Business;

public class FilterParameters : IFilterParameters
{
    public List<Guid> EquipmentIds { get; set; }
    public DateTime TimeFrom { get; set; }
    public DateTime TimeTo { get; set; }

    public IDictionary<string, string> ToDictionary()
    {
        var dict = new Dictionary<string, string>
        {
            { nameof(TimeFrom), TimeFrom.ToString("u") },
            { nameof(TimeTo), TimeTo.ToString("u") }
        };

        for (int i = 0; i < EquipmentIds.Count; i++)
        {
            dict.Add($"{nameof(EquipmentIds)}[{i}]", EquipmentIds[i].ToString("N"));
        };

        return dict;
    }
}