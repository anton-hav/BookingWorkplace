using BookingWorkplace.Core.Abstractions;

namespace BookingWorkplace.Business;

/// <summary>
///     Class is the container for the actual pagination parameters.
/// </summary>
public class PaginationParameters : IPaginationParameters
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    /// <summary>
    ///     Contains information about the page number. The default value is 1
    /// </summary>
    public int CurrentPage { get; set; } = 1;

    /// <summary>
    ///     Contains information about the number of elements on the page. The maximum value is 100
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value is > 0 and > MaxPageSize ? MaxPageSize : value;
    }

    public IDictionary<string, string> ToDictionary()
    {
        return new Dictionary<string, string>
        {
            { nameof(CurrentPage), CurrentPage.ToString() },
            { nameof(PageSize), PageSize.ToString() }
        };
    }
}