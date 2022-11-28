using System.Text;
using BookingWorkplace.Core;
using BookingWorkplace.Core.Abstractions;
using BookingWorkplace.Core.DataTransferObjects;

namespace BookingWorkplace.Business.ServiceImplementations;

public class BookingEventHandler : IBookingEventHandler
{
    private readonly ISender _sender;
    private readonly IWorkplaceService _workplaceService;
    private readonly IEquipmentForWorkplaceService _equipmentForWorkplaceService;

    public BookingEventHandler(ISender sender,
        IWorkplaceService workplaceService, IEquipmentForWorkplaceService equipmentForWorkplaceService)
    {
        _sender = sender;
        _workplaceService = workplaceService;
        _equipmentForWorkplaceService = equipmentForWorkplaceService;
    }

    /// <summary>
    ///     Sends information about new user registration.
    /// </summary>
    /// <param name="email">user email as a string</param>
    /// <returns>The Task</returns>
    public async Task ReportNewUserRegistration(string email)
    {
        var message = $"A new user signed up using {email}";
        await _sender.SendMessageAsync(message);
    }

    /// <summary>
    ///     Sends information about new reservation.
    /// </summary>
    /// <param name="email">email of the user who made the reservation as a string</param>
    /// <param name="workplace">information about the reserved workplace as a <see cref="WorkplaceDto" /></param>
    /// <param name="reservation">information about the reservation as a <see cref="ReservationDto" /></param>
    /// <returns>The Task</returns>
    public async Task ReportNewReservationAsync(string email, WorkplaceDto workplace, ReservationDto reservation)
    {
        var message =
            $"User with {email} reserved a workstation on {workplace.Floor} floor in Room {workplace.Room} from {reservation.TimeFrom.ToString("D")} to {reservation.TimeFrom.ToString("D")}";
        await _sender.SendMessageAsync(message);
    }

    /// <summary>
    ///     Sends information about the movement of equipment.
    /// </summary>
    /// <param name="movement">a movement information as a <see cref="EquipmentMovementData" /></param>
    /// <returns>The Task</returns>
    public async Task ReportEquipmentMovementAsync(EquipmentMovementData movement)
    {
        var equipment =
            await _equipmentForWorkplaceService.GetEquipmentByEquipmentForWorkplaceIdAsync(movement.EquipmentId);
        var previousWorkplace = await _workplaceService.GetWorkplaceByIdAsync(movement.PreviousWorkplace);
        var destinationWorkplace = await _workplaceService.GetWorkplaceByIdAsync(movement.DestinationWorkplace);

        var sb = new StringBuilder();

        sb.Append($"Equipment {equipment.Type} must be moved ");
        sb.Append(
            $"from {previousWorkplace.Floor} floor, {previousWorkplace.Room} room, {previousWorkplace.DeskNumber} desk number ");
        sb.Append(
            $"to {destinationWorkplace.Floor} floor, {destinationWorkplace.Room} room, {destinationWorkplace.DeskNumber} desk number ");
        sb.Append($"at the end of the day on {movement.DateOfMovement.ToString("D")}.");

        await _sender.SendMessageAsync(sb.ToString());
    }
}