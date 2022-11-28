using BookingWorkplace.Core.Abstractions;

namespace BookingWorkplace.Core;

public class PagedList<T> : List<T>
{
    public int CurrentPage { get; }
    public int TotalPages { get; }
    public int PageSize { get; }
    public int TotalCount { get; }

    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    /// <summary>
    ///     Constructor without any parameters
    /// </summary>
    /// <remarks>
    ///     It added for compatibility with AutoMapper and others utils.
    ///     Don't use it to create new instance directly.
    /// </remarks>
    public PagedList()
    {
    }

    public PagedList(List<T> items,
        int count,
        int pageNumber,
        int pageSize)
    {
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        AddRange(items);
    }

    public static PagedList<T> ToPagedList(IQueryable<T> source, IPaginationParameters parameters)
    {
        var count = source.Count();
        var items = source
            .Skip((parameters.CurrentPage - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToList();

        return new PagedList<T>(items, count, parameters.CurrentPage, parameters.PageSize);
    }
}