using AutoMapper;
using BookingWorkplace.Business;
using BookingWorkplace.Core;
using BookingWorkplace.Core.Abstractions;
using BookingWorkplace.Core.DataTransferObjects;
using BookingWorkplace.Models;
using BookingWorkplace.SessionUtils;
using BookingWorkplace.SessionUtils.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace BookingWorkplace.Controllers;

[Authorize(Roles = "User, Admin")]
public class ReservationController : Controller
{
    private readonly IMapper _mapper;
    private readonly IUserManager _userManager;
    private readonly IReservationService _reservationService;
    private readonly IEquipmentService _equipmentService;
    private readonly IWorkplaceService _workplaceService;
    private readonly ISessionManager _sessionManager;
    private readonly IEquipmentForWorkplaceService _equipmentForWorkplaceService;
    private readonly IBookingEventHandler _bookingEventHandler;

    public ReservationController(IMapper mapper,
        IUserManager userManager,
        IReservationService reservationService,
        IEquipmentService equipmentService,
        IWorkplaceService workplaceService,
        ISessionManager sessionManager,
        IEquipmentForWorkplaceService equipmentForWorkplaceService,
        IBookingEventHandler bookingEventHandler)
    {
        _mapper = mapper;
        _userManager = userManager;
        _reservationService = reservationService;
        _equipmentService = equipmentService;
        _workplaceService = workplaceService;
        _sessionManager = sessionManager;
        _equipmentForWorkplaceService = equipmentForWorkplaceService;
        _bookingEventHandler = bookingEventHandler;
    }

    public async Task<IActionResult> Index(QueryStringParameters parameters)
    {
        try
        {
            var isUserAdmin = _userManager.IsAdmin();

            var userId = await _userManager.GetUserIdAsync();

            var reservations = isUserAdmin
                ? _reservationService
                    .GetReservationsByQueryStringParameters(parameters)
                : _reservationService
                    .GetReservationsByQueryStringParameters(parameters, userId);

            var model = new ListOfReservationsModel
            {
                Reservations = reservations,
                SearchString = parameters,
                IsAdmin = isUserAdmin
            };

            return View(model);
        }
        catch (Exception ex)
        {
            Log.Error($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
            return StatusCode(500);
        }
    }

    [Authorize(Roles = "User")]
    [HttpGet]
    public async Task<IActionResult> PreBooking([FromQuery] FilterParameters filters)
    {
        try
        {
            // checks and initiates new session
            var isExist = await _sessionManager.IsSessionExistAsync();

            if (!isExist)
            {
                var userId = await _userManager.GetUserIdAsync();
                var reservationSession = new ReservationSession
                {
                    ReservationId = Guid.NewGuid(),
                    UserId = userId,
                    WorkplaceId = Guid.Empty,
                    TimeFrom = filters.TimeFrom.Equals(default) ? DateTime.Today : filters.TimeFrom,
                    TimeTo = filters.TimeTo.Equals(default) ? DateTime.Today : filters.TimeTo,
                    EquipmentIds = filters.EquipmentIds.IsNullOrEmpty() ? new List<Guid>() : filters.EquipmentIds
                };

                await _sessionManager.SetSessionAsync(reservationSession);
            }

            // checks and initiates value for date fields of filters
            var session = await _sessionManager.GetSessionAsync();

            if (filters.TimeFrom.Equals(default))
                filters.TimeFrom = session.TimeFrom;
            else
                session.TimeFrom = filters.TimeFrom;

            if (filters.TimeTo.Equals(default))
                filters.TimeTo = session.TimeTo;
            else
                session.TimeTo = filters.TimeTo;

            if (filters.EquipmentIds.IsNullOrEmpty()) filters.EquipmentIds = new List<Guid>();

            session.EquipmentIds = filters.EquipmentIds;

            await _sessionManager.SetSessionAsync(session);

            // generates the list of select items for the filter bar
            var equipmentList = await _equipmentService.GetAllEquipmentAsync();
            var equipmentSelectList = equipmentList.Select(equip => new SelectListItem
            {
                Text = equip.Type,
                Value = equip.Id.ToString("N"),
                Selected = filters.EquipmentIds != null && filters.EquipmentIds.Contains(equip.Id)
            }).ToList();

            // gets the list of relevant workplaces
            var workplaces = _workplaceService.GetWorkplacesByFilterParameters(filters);

            // prepares model
            var model = new PreBookingModel
            {
                Filters = filters,
                EquipmentList = equipmentSelectList,
                Workplaces = workplaces,
                UnderstaffedWorkplaces = new List<WorkplaceDto>()
            };

            // gets the list of understaffed workplaces
            var understaffedWorkplaces = await _workplaceService
                .GetPossibleWorkplacesByFilterParameters(filters, workplaces);

            // checks whether it is possible to form a pool of necessary equipment
            if (understaffedWorkplaces.Any())
            {
                var isEquipmentPoolPossible = await _equipmentForWorkplaceService
                    .IsPossibleToFindNecessaryEquipmentToMoveAsync(filters);

                if (isEquipmentPoolPossible) model.UnderstaffedWorkplaces.AddRange(understaffedWorkplaces);
            }

            return View(model);
        }
        catch (ArgumentException ex)
        {
            Log.Warning($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
            return BadRequest();
        }
        catch (Exception ex)
        {
            Log.Error($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
            return StatusCode(500);
        }
    }

    [Authorize(Roles = "User")]
    [HttpPost]
    public async Task<IActionResult> Create(Guid id)
    {
        try
        {
            if (id.Equals(Guid.Empty))
                throw new ArgumentException(nameof(id));

            var reservation = await CollectDataForNewReservation(id);

            var isValid = await IsReservationValid(reservation.WorkplaceId, reservation.TimeFrom, reservation.TimeTo);

            if (!isValid)
                throw new ArgumentException("A reservation with current parameters does not valid.");

            var session = await _sessionManager.GetSessionAsync();

            var relocatedEquipment = await _equipmentForWorkplaceService
                .GetMovableEquipmentForWorkplaceAsync(session);

            var equipmentMovements = new List<EquipmentMovementData>();

            foreach (var equip in relocatedEquipment)
            {
                var movement = await _equipmentForWorkplaceService
                    .GetEquipmentMovementDataAsync(equip.Id, id);

                equipmentMovements.Add(movement);
            }

            await _equipmentForWorkplaceService
                .PrepareEquipmentForRelocationToWorkplaceAsync(relocatedEquipment, id);

            var result = await _reservationService.CreateReservationAsync(reservation);
            if (result > 0)
            {
                await HandleNewReservation(reservation);
                await HandleRelocationEquipment(equipmentMovements);
                await _sessionManager.RemoveSessionAsync();
            }

            return RedirectToAction("Index", "Reservation");
            ;
        }
        catch (ArgumentException ex)
        {
            Log.Warning($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
            return BadRequest();
        }
        catch (Exception ex)
        {
            Log.Error($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
            return StatusCode(500);
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            if (id.Equals(Guid.Empty))
                throw new ArgumentException(nameof(id));

            var dto = await _reservationService.GetReservationByIdAsync(id);

            var userId = await _userManager.GetUserIdAsync();

            if (dto.UserId.Equals(userId))
                await _reservationService.DeleteAsync(dto);

            return RedirectToAction("Index", "Reservation");
        }
        catch (ArgumentException ex)
        {
            Log.Warning($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
            return BadRequest();
        }
        catch (Exception ex)
        {
            Log.Error($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
            return StatusCode(500);
        }
    }


    /// <summary>
    ///     Checks for existing reservations with the same parameters
    ///     or reservations for the current user with a time interval overlapping the parameters.
    /// </summary>
    /// <param name="workplaceId">a workplace unique identifier as a <see cref="Guid" /></param>
    /// <param name="timeFrom">a check in time as a <see cref="DateTime" /></param>
    /// <param name="timeTo">a check out time as a <see cref="DateTime" /></param>
    /// <returns>A boolean (true if the reservation does not exist, or false if it exists)</returns>
    private async Task<bool> IsReservationValid(Guid workplaceId, DateTime timeFrom, DateTime timeTo)
    {
        var isReservationExist = await _reservationService.IsReservationExistAsync(workplaceId, timeFrom, timeTo);

        var userId = await _userManager.GetUserIdAsync();

        var isReservationForUserExist =
            await _reservationService.IsReservationForUserExistAsync(userId, timeFrom, timeTo);

        return !isReservationExist && !isReservationForUserExist;
    }

    /// <summary>
    ///     Gets data from the storage for a new reservation.
    /// </summary>
    /// <param name="workplaceId">a workplace unique identifier as a <see cref="Guid" /></param>
    /// <returns>
    ///     <see cref="ReservationDto" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    private async Task<ReservationDto> CollectDataForNewReservation(Guid workplaceId)
    {
        var isSessionExist = await _sessionManager.IsSessionExistAsync();
        if (!isSessionExist)
            throw new ArgumentException("Session not found. Possibly the lifetime of the session has been exceeded.");

        var session = await _sessionManager.GetSessionAsync();
        var userId = await _userManager.GetUserIdAsync();

        var reservation = new ReservationDto
        {
            Id = session.ReservationId,
            UserId = userId,
            WorkplaceId = workplaceId,
            TimeFrom = session.TimeFrom,
            TimeTo = session.TimeTo
        };

        return reservation;
    }

    /// <summary>
    ///     Prepares information and initiates inform event processing for a new reservation.
    /// </summary>
    /// <param name="reservation">
    ///     <see cref="ReservationDto" />
    /// </param>
    /// <returns>The Task</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    private async Task HandleNewReservation(ReservationDto reservation)
    {
        var email = (await _userManager.GetUserAsync()).Email;
        var workplace = await _workplaceService.GetWorkplaceByIdAsync(reservation.WorkplaceId);

        if (email.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(email));

        if (workplace == null)
            throw new ArgumentNullException(nameof(workplace));

        await _bookingEventHandler.ReportNewReservationAsync(email, workplace, reservation);
    }

    private async Task HandleRelocationEquipment(List<EquipmentMovementData> movements)
    {
        foreach (var movement in movements) await _bookingEventHandler.ReportEquipmentMovementAsync(movement);
    }
}