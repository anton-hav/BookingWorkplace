using BookingWorkplace.Core.Abstractions;
using BookingWorkplace.Core.DataTransferObjects;
using BookingWorkplace.Core;

namespace BookingWorkplace.Models;

public class ListOfReservationsModel
{
    public PagedList<ReservationDto> Reservations { get; set; }
    public ISearchString SearchString { get; set; }
    public bool IsAdmin { get; set; }
}