using BookingWorkplace.Core;
using BookingWorkplace.Core.Abstractions;
using BookingWorkplace.Core.DataTransferObjects;

namespace BookingWorkplace.Models;

public class ListOfEquipmentModel
{
    public PagedList<EquipmentDto> Equipment { get; set; }
    public ISearchString SearchString { get; set; }
    public bool IsAdmin { get; set; }
}