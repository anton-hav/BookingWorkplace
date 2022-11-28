namespace BookingWorkplace.Core.Abstractions;

public interface ISearchString
{
    string SearchString { get; set; }
    IDictionary<string, string> ToDictionary();
}