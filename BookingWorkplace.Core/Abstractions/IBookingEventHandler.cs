using BookingWorkplace.Core.DataTransferObjects;

namespace BookingWorkplace.Core.Abstractions;

public interface IBookingEventHandler
{
    Task ReportNewUserRegistration(string email);
    Task ReportNewReservationAsync(string email, WorkplaceDto workplace, ReservationDto reservation);
    Task ReportEquipmentMovementAsync(EquipmentMovementData movement);
}