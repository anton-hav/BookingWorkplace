using BookingWorkplace.Core.Abstractions;

namespace BookingWorkplace.Business;

/// <summary>
///     Filter options for finding a suitable reservation.
/// </summary>
public class FilterParameters : IFilterParameters
{
    /// <summary>
    ///     List of required equipment.
    /// </summary>
    public List<Guid> EquipmentIds { get; set; }

    /// <summary>
    ///     Check in date
    /// </summary>
    public DateTime TimeFrom { get; set; }

    /// <summary>
    ///     Check out date
    /// </summary>
    public DateTime TimeTo { get; set; }

    /// <summary>
    ///     Converter object to IDictionary
    /// </summary>
    /// <returns><see cref="IDictionary{TKey,TValue}" /> where TKey is the property name, TValue is the property value</returns>
    public IDictionary<string, string> ToDictionary()
    {
        var dict = new Dictionary<string, string>
        {
            { nameof(TimeFrom), TimeFrom.ToString("u") },
            { nameof(TimeTo), TimeTo.ToString("u") }
        };

        for (var i = 0; i < EquipmentIds.Count; i++)
            dict.Add($"{nameof(EquipmentIds)}[{i}]", EquipmentIds[i].ToString("N"));
        ;

        return dict;
    }
}