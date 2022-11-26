using AutoMapper;
using BookingReservation.Core.Abstractions;
using BookingWorkplace.Business;
using BookingWorkplace.Core.Abstractions;
using BookingWorkplace.Core.DataTransferObjects;
using BookingWorkplace.DataBase.Entities;
using BookingWorkplace.Models;
using BookingWorkplace.SessionUtils;
using BookingWorkplace.SessionUtils.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using ReservationSession = BookingWorkplace.SessionUtils.ReservationSession;

namespace BookingWorkplace.Controllers
{
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

        public ReservationController(IMapper mapper,
            IUserManager userManager,
            IReservationService reservationService,
            IEquipmentService equipmentService,
            IWorkplaceService workplaceService, 
            ISessionManager sessionManager, 
            IEquipmentForWorkplaceService equipmentForWorkplaceService)
        {
            _mapper = mapper;
            _userManager = userManager;
            _reservationService = reservationService;
            _equipmentService = equipmentService;
            _workplaceService = workplaceService;
            _sessionManager = sessionManager;
            _equipmentForWorkplaceService = equipmentForWorkplaceService;
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

                var model = new ListOfReservationsModel()
                {
                    Reservations = reservations,
                    SearchString = parameters,
                    IsAdmin = isUserAdmin,
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
                        EquipmentIds = filters.Ids.IsNullOrEmpty() ? new List<Guid>() : filters.Ids
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

                if (filters.Ids.IsNullOrEmpty())
                {
                    filters.Ids = new List<Guid>();
                }

                session.EquipmentIds = filters.Ids;

                await _sessionManager.SetSessionAsync(session);

                // generates the list of select items for the filter bar
                var equipmentList = await _equipmentService.GetAllEquipmentAsync();
                var equipmentSelectList = equipmentList.Select(equip => new SelectListItem
                {
                    Text = equip.Type,
                    Value = equip.Id.ToString("N"),
                    Selected = filters.Ids != null && filters.Ids.Contains(equip.Id)
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

                    if (isEquipmentPoolPossible)
                    {
                        model.UnderstaffedWorkplaces.AddRange(understaffedWorkplaces);
                    }
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
                
                var isExist = await _sessionManager.IsSessionExistAsync();
                if (!isExist)
                    throw new ArgumentException("Session not found. Possibly the lifetime of the session has been exceeded.");

                var session = await _sessionManager.GetSessionAsync();
                var userId = await _userManager.GetUserIdAsync();

                var dto = new ReservationDto()
                {
                    Id = session.ReservationId,
                    UserId = userId,
                    WorkplaceId = id,
                    TimeFrom = session.TimeFrom,
                    TimeTo = session.TimeTo,
                };

                var isValid = await IsReservationValid(dto.WorkplaceId, dto.TimeFrom, dto.TimeTo);

                if (!isValid)
                    throw new ArgumentException("A reservation with current parameters does not valid.");


                var filters = new FilterParameters()
                {
                    Ids = session.EquipmentIds,
                    TimeFrom = session.TimeFrom,
                    TimeTo = session.TimeTo,
                };

                var relocatedEquipment = await _equipmentForWorkplaceService
                    .GetMovableEquipmentForWorkplaceAsync(filters);

                await _equipmentForWorkplaceService
                    .PrepareEquipmentForRelocationToWorkplaceAsync(relocatedEquipment, id);

                var result = await _reservationService.CreateReservationAsync(dto);
                if (result.Equals(1))
                    await _sessionManager.RemoveSessionAsync();

                return RedirectToAction("Index", "Reservation"); ;
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

        [HttpPost]
        public async Task<IActionResult> ValidateReservation(Guid workplaceId, DateTime timeFrom, DateTime timeTo)
        {
            try
            {
                var isValid = await IsReservationValid(workplaceId, timeFrom, timeTo);

                return Ok(isValid);
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
        /// Checks for existing reservations with the same parameters
        /// or reservations for the current user with a time interval overlapping the parameters. 
        /// </summary>
        /// <param name="workplaceId">a workplace unique identifier as a <see cref="Guid"/></param>
        /// <param name="timeFrom">a check in time as a <see cref="DateTime"/></param>
        /// <param name="timeTo">a check out time as a <see cref="DateTime"/></param>
        /// <returns>A boolean (true if the reservation does not exist, or false if it exists)</returns>
        private async Task<bool> IsReservationValid(Guid workplaceId, DateTime timeFrom, DateTime timeTo)
        {
            var isReservationExist = await _reservationService.IsReservationExistAsync(workplaceId, timeFrom, timeTo);

            var userId = await _userManager.GetUserIdAsync();

            var isReservationForUserExist =
                await _reservationService.IsReservationForUserExistAsync(userId, timeFrom, timeTo);

            return !isReservationExist && !isReservationForUserExist;
        }
    }
}
