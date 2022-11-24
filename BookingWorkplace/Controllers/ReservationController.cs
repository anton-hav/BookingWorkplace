using AutoMapper;
using BookingReservation.Core.Abstractions;
using BookingWorkplace.Business;
using BookingWorkplace.Core.Abstractions;
using BookingWorkplace.Core.DataTransferObjects;
using BookingWorkplace.DataBase.Entities;
using BookingWorkplace.Models;
using BookingWorkplace.SessionUtils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        private const int SessionKeyOffset = 5;

        public ReservationController(IMapper mapper,
            IUserManager userManager,
            IReservationService reservationService,
            IEquipmentService equipmentService,
            IWorkplaceService workplaceService)
        {
            _mapper = mapper;
            _userManager = userManager;
            _reservationService = reservationService;
            _equipmentService = equipmentService;
            _workplaceService = workplaceService;
        }

        public IActionResult Index()
        {
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> PreBooking([FromQuery] FilterParameters filters)
        {
            try
            {
                // checks and initiates new session
                var userId = await _userManager.GetUserIdAsync();
                var sessionKey = userId.ToString("N").Substring(SessionKeyOffset);
                var isSucceed =
                    HttpContext.Session.TryGetValue<ReservationSession>(sessionKey, out var reservationSession);

                if (!isSucceed)
                {
                    reservationSession = new ReservationSession
                    {
                        ReservationId = Guid.NewGuid(),
                        UserId = userId,
                        WorkplaceId = Guid.Empty,
                        TimeFrom = filters.TimeFrom.Equals(DateTime.MinValue) ? DateTime.UtcNow : filters.TimeFrom,
                        TimeTo = filters.TimeTo.Equals(default) ? DateTime.UtcNow : filters.TimeTo
                    };

                    HttpContext.Session.Set(sessionKey, reservationSession);
                }

                // checks and initiates value for date fields of filters
                if (filters.TimeFrom.Equals(default))
                    filters.TimeFrom = reservationSession.TimeFrom;
                else
                    reservationSession.TimeFrom = filters.TimeFrom;

                if (filters.TimeTo.Equals(default))
                    filters.TimeTo = reservationSession.TimeTo;
                else
                    reservationSession.TimeTo = filters.TimeTo;

                if (filters.Ids.IsNullOrEmpty()) filters.Ids = new List<Guid>();

                HttpContext.Session.Set(sessionKey, reservationSession);

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
                    Workplaces = workplaces
                };
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

        [HttpPost]
        public async Task<IActionResult> Create(Guid id)
        {
            try
            {
                if (id.Equals(Guid.Empty))
                    throw new ArgumentException(nameof(id));

                var userId = await _userManager.GetUserIdAsync();
                var sessionKey = userId.ToString("N").Substring(SessionKeyOffset);
                var isSucceed =
                    HttpContext.Session.TryGetValue<ReservationSession>(sessionKey, out var reservationSession);

                if (!isSucceed)
                    throw new ArgumentException("Session not found. Possibly the lifetime of the session has been exceeded.");

                var dto = new ReservationDto()
                {
                    Id = reservationSession.ReservationId,
                    UserId = userId,
                    WorkplaceId = id,
                    TimeFrom = reservationSession.TimeFrom,
                    TimeTo = reservationSession.TimeTo,
                };

                var result = await _reservationService.CreateReservationAsync(dto);

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
    }
}
