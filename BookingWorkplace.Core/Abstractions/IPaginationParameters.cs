namespace BookingWorkplace.Core.Abstractions;

public interface IPaginationParameters
{
    int CurrentPage { get; set; }
    int PageSize { get; set; }

    IDictionary<string, string> ToDictionary();
}