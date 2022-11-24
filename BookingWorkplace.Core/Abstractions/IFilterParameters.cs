namespace BookingWorkplace.Core.Abstractions;

public interface IFilterParameters
{
    List<Guid> Ids { get; set; }
    DateTime TimeFrom { get; set; }
    DateTime TimeTo { get; set; }

    IDictionary<string, string> ToDictionary();
}