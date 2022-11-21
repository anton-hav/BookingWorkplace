using BookingWorkplace.Core.Abstractions;

namespace BookingWorkplace.Business;

/// <summary>
/// The class is a container for the actual pagination parameters and the search string.
/// </summary>
public class QueryStringParameters : IQueryStringParameters//IPaginationParameters, ISearchString
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    /// <summary>
    /// Contains information about the page number. The default value is 1
    /// </summary>
    public int CurrentPage { get; set; } = 1;

    /// <summary>
    /// Contains information about the number of elements on the page. The maximum value is 100
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value is > 0 and > MaxPageSize ? MaxPageSize : value;
    }

    /// <summary>
    /// The search string value is used to filter queries to the data source.
    /// The default value is String.Empty
    /// </summary>
    public string SearchString { get; set; } = String.Empty;

    /// <summary>
    /// Creates a IDictionary &lt;TKey,TValue&gt; from an object.
    /// Where &lt;TKey&gt; is the name of the property,
    /// &lt;TValue&gt; is the value of the corresponding property
    /// </summary>
    /// <returns>the dictionary of all properties</returns>
    public IDictionary<string, string> ToDictionary()
    {
        return new Dictionary<string, string>
        {
            { nameof(CurrentPage), CurrentPage.ToString() },
            { nameof(PageSize), PageSize.ToString() },
            { nameof(SearchString), SearchString }
        };
    }
}