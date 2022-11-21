using BookingWorkplace.Core.Abstractions;

namespace BookingWorkplace.Core;

public class PagedList<T> : List<T>
{
    public int CurrentPage { get; private set; }
    public int TotalPages { get; private set; }
    public int PageSize { get; private set; }
    public int TotalCount { get; private set; }

    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

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