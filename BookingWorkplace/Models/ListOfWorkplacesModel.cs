using BookingWorkplace.Core.Abstractions;
using BookingWorkplace.Core.DataTransferObjects;
using BookingWorkplace.Core;

namespace BookingWorkplace.Models;

public class ListOfWorkplacesModel
{
    public PagedList<WorkplaceDto> Workplaces { get; set; }
    public ISearchString SearchString { get; set; }
}