using AutoMapper;
using BookingWorkplace.Business;
using BookingWorkplace.Core.Abstractions;
using BookingWorkplace.Core.DataTransferObjects;
using BookingWorkplace.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Serilog;

namespace BookingWorkplace.Controllers;

[Authorize(Roles = "User, Admin")]
public class WorkplaceController : Controller
{
    private readonly IMapper _mapper;
    private readonly IWorkplaceService _workplaceService;
    private readonly IEquipmentService _equipmentService;
    private readonly IEquipmentForWorkplaceService _equipmentForWorkplaceService;
    private readonly IUserManager _userManager;

    public WorkplaceController(IMapper mapper,
        IWorkplaceService workplaceService,
        IEquipmentService equipmentService,
        IEquipmentForWorkplaceService equipmentForWorkplaceService,
        IUserManager userManager)
    {
        _mapper = mapper;
        _workplaceService = workplaceService;
        _equipmentService = equipmentService;
        _equipmentForWorkplaceService = equipmentForWorkplaceService;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Index(QueryStringParameters parameters)
    {
        try
        {
            var workplaces = _workplaceService
                .GetWorkplacesByQueryStringParameters(parameters);

            var isUserAdmin = _userManager.IsAdmin();

            var model = new ListOfWorkplacesModel
            {
                Workplaces = workplaces,
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

    /// <summary>
    ///     Shows the create page
    /// </summary>
    /// <returns>ViewResult for response</returns>
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult Create()
    {
        try
        {
            return View();
        }
        catch (Exception ex)
        {
            Log.Error($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
            return StatusCode(500);
        }
    }

    /// <summary>
    ///     Endpoint that create the workplace record in the data source.
    /// </summary>
    /// <param name="model">workplace model</param>
    /// <returns>redirect to /Workplace/Index if model is valid</returns>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(WorkplaceModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                model.Id = Guid.NewGuid();

                var dto = _mapper.Map<WorkplaceDto>(model);

                var result = await _workplaceService.CreateWorkplaceAsync(dto);
                return RedirectToAction("Index", "Workplace");
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

    /// <summary>
    ///     Shows the edit page
    /// </summary>
    /// <param name="id">unique identifier of the object to be changed</param>
    /// <returns>ViewResult for response</returns>
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            if (id == Guid.Empty) throw new ArgumentException(nameof(id));

            var dto = await _workplaceService.GetWorkplaceByIdAsync(id);

            var editModel = _mapper.Map<WorkplaceModel>(dto);

            return View(editModel);
        }
        catch (ArgumentException ex)
        {
            Log.Error($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
            return BadRequest();
        }
        catch (Exception ex)
        {
            Log.Error($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
            return StatusCode(500);
        }
    }

    /// <summary>
    ///     Endpoint that changes the workplace record in the data source.
    /// </summary>
    /// <param name="model">modified model</param>
    /// <returns>redirect to /Workplace/Index if model is valid</returns>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Edit(WorkplaceModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var dto = _mapper.Map<WorkplaceDto>(model);
                await _workplaceService.UpdateAsync(model.Id, dto);

                return RedirectToAction("Index", "Workplace");
            }

            return View(model);
        }
        catch (ArgumentException ex)
        {
            Log.Error($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
            return BadRequest();
        }
        catch (Exception ex)
        {
            Log.Error($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
            return StatusCode(500);
        }
    }

    /// <summary>
    ///     Checks the workplace for existing workplace with the same room number
    ///     and floor number and desk number in the data source.
    /// </summary>
    /// <param name="deskNumber">desk number of the current workplace as a string</param>
    /// <param name="floor">floor number of the current workplace as a string</param>
    /// <param name="room">room number of the current workplace as a string</param>
    /// <returns>OkObjectResult with true if the equipment is does not exist or false if the equipment exist</returns>
    [HttpPost]
    public async Task<IActionResult> CheckWorkplaceForExistence(string deskNumber, string floor, string room)
    {
        try
        {
            var isValid = await _workplaceService.IsWorkplaceExistAsync(room, floor, deskNumber);
            return Ok(!isValid);
        }
        catch (ArgumentException ex)
        {
            Log.Error($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
            return BadRequest();
        }
        catch (Exception ex)
        {
            Log.Error($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
            return StatusCode(500);
        }
    }

    /// <summary>
    ///     Shows the details page
    /// </summary>
    /// <param name="id">unique identifier</param>
    /// <returns>ViewResult for response</returns>
    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var dto = await _workplaceService.GetWorkplaceWithEquipmentByIdAsync(id);

            var model = _mapper.Map<WorkplaceWithEquipmentModel>(dto);

            model.IsAdmin = _userManager.IsAdmin();

            var equipmentList = await _equipmentService.GetAvailableEquipmentToAddToWorkplaceByWorkplaceIdAsync(id);

            model.AvailableEquipmentList =
                new SelectList(equipmentList, nameof(EquipmentDto.Id), nameof(EquipmentDto.Type));

            return View(model);
        }
        catch (ArgumentException ex)
        {
            Log.Error($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
            return BadRequest();
        }
        catch (Exception ex)
        {
            Log.Error($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
            return StatusCode(500);
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddEquipmentToWorkplace(EquipmentForWorkplaceModel model)
    {
        try
        {
            model.Id = Guid.NewGuid();
            var dto = _mapper.Map<EquipmentForWorkplaceDto>(model);

            var result = await _equipmentForWorkplaceService.CreateEquipmentForWorkplaceAsync(dto);
            return RedirectToAction("Details", "Workplace", new { id = dto.WorkplaceId });
        }
        catch (ArgumentException ex)
        {
            Log.Error($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
            return BadRequest();
        }
        catch (Exception ex)
        {
            Log.Error($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
            return StatusCode(500);
        }
    }

    [HttpGet]
    public async Task<IActionResult> RemoveEquipmentFromWorkplace(Guid id)
    {
        try
        {
            if (!id.Equals(Guid.Empty))
            {
                var dto = await _equipmentForWorkplaceService.GetEquipmentForWorkplaceByIdAsync(id);
                await _equipmentForWorkplaceService.DeleteAsync(id);
                return RedirectToAction("Details", "Workplace", new { id = dto.WorkplaceId });
            }

            return Ok();
        }
        catch (ArgumentException ex)
        {
            Log.Error($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
            return BadRequest();
        }
        catch (Exception ex)
        {
            Log.Error($"{ex.Message}. {Environment.NewLine} {ex.StackTrace}");
            return StatusCode(500);
        }
    }
}