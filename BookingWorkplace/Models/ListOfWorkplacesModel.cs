using BookingWorkplace.Core;
using BookingWorkplace.Core.Abstractions;
using BookingWorkplace.Core.DataTransferObjects;

namespace BookingWorkplace.Models;

public class ListOfWorkplacesModel
{
    public PagedList<WorkplaceDto> Workplaces { get; set; }
    public ISearchString SearchString { get; set; }
    public bool IsAdmin { get; set; }
}