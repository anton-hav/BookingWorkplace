using AutoMapper;
using BookingWorkplace.Business;
using BookingWorkplace.Core.Abstractions;
using BookingWorkplace.Core.DataTransferObjects;
using BookingWorkplace.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace BookingWorkplace.Controllers
{
    [Authorize(Roles = "User, Admin")]
    public class EquipmentController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IEquipmentService _equipmentService;
        private readonly IUserManager _userManager;

        public EquipmentController(IMapper mapper, 
            IEquipmentService equipmentService, 
            IUserManager userManager)
        {
            _mapper = mapper;
            _equipmentService = equipmentService;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index(QueryStringParameters parameters)
        {
            try
            {
                var listOfEquipment = _equipmentService
                    .GetEquipmentByQueryStringParameters(parameters);

                var isUserAdmin = _userManager.IsAdmin();
                
                var model = new ListOfEquipmentModel()
                {
                    Equipment = listOfEquipment,
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

        /// <summary>
        /// Shows the create page
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
        /// Endpoint that create the equipment record in the data source.
        /// </summary>
        /// <param name="model">equipment model</param>
        /// <returns>redirect to /Equipment/Index if model is valid</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(EquipmentModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.Id = Guid.NewGuid();

                    var dto = _mapper.Map<EquipmentDto>(model);
                    
                    var result = await _equipmentService.CreateEquipmentAsync(dto);
                    return RedirectToAction("Index", "Equipment");
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
        /// Shows the edit page
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

                var dto = await _equipmentService.GetEquipmentByIdAsync(id);

                var editModel = _mapper.Map<EquipmentModel>(dto);

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
        /// Endpoint that changes the equipment record in the data source.
        /// </summary>
        /// <param name="model">modified model</param>
        /// <returns>redirect to /Equipment/Index if model is valid</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(EquipmentModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var dto = _mapper.Map<EquipmentDto>(model);
                    await _equipmentService.UpdateAsync(model.Id, dto);

                    return RedirectToAction("Index", "Equipment");

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
        /// Shows the details page
        /// </summary>
        /// <param name="id">the equipment unique identifier</param>
        /// <returns><see cref="ViewResult"/> for response</returns>
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var dto = await _equipmentService.GetEquipmentWithFullInfoByIdAsync(id);

                var model = _mapper.Map<EquipmentDetailModel>(dto);

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
        /// Checks the equipment for existing equipment with the same type name in the data source.
        /// </summary>
        /// <param name="type">type name of equipment</param>
        /// <returns>OkObjectResult with true if the equipment is does not exist or false if the equipment exist</returns>
        [HttpPost]
        public async Task<IActionResult> CheckEquipmentForExistence(string type)
        {
            try
            {
                var isValid = await _equipmentService.IsEquipmentExistAsync(type);
                return Ok(!isValid);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }

}
